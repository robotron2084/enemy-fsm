using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.enemyhideout.fsm.tests
{
  [TestFixture]
  public partial class TestFsm
  {
    void AssertLog(List<string> objectHistory, params string[] args)
    {
      Assert.True(objectHistory.Count == args.Length, "History and expected history values do not match up.");
      for (int i = 0; i < args.Length; i++)
      {
        Assert.AreEqual(args[i], objectHistory[i]);
      }
      objectHistory.Clear();
    }

    [UnityTest]
    public IEnumerator TestFsmChangeState() => UniTask.ToCoroutine(async () =>
    {
      try
      {
        TestActor actor = new TestActor();
        actor.fsm.ChangeState(TestActor.States.Sprint);
        await Task.Delay(1100);
        AssertLog(actor.ObjectHistory, "Entering Sprint...", "Sprint Entered!");
        
        actor.fsm.ChangeState(TestActor.States.Idle);
        actor.fsm.ChangeState(TestActor.States.Sprint);
        await Task.Delay(100);
        //changing state! we should not see log messages!
        actor.fsm.ChangeState(TestActor.States.Idle);
        await Task.Delay(1000);
        AssertLog(actor.ObjectHistory, "Entering Sprint...", "Exiting Sprint Early.");

        actor.fsm.ChangeState(TestActor.States.Running);
        await Task.Delay(1000);
        AssertLog(actor.ObjectHistory,"Ready...", "Set...", "Go!!!",
          "Running 0","Running 1","Running 2","Running 3","Running 4");

        actor.fsm.ChangeState(TestActor.States.Sprint);
        await Task.Delay(1100);
        AssertLog(actor.ObjectHistory, "Exiting Run...","Done running.", "Entering Sprint...", "Sprint Entered!");

        actor.fsm.ChangeState(TestActor.States.Hiding);
        await Task.Delay(10);
        AssertLog(actor.ObjectHistory, "Hiding");
        
        actor.fsm.ChangeState(TestActor.States.DoubleJump);
        await UniTask.Yield();
        AssertLog(actor.ObjectHistory, "DoubleJump", "DoubleJump2", "DoubleJump3");

        // testing changing state while in update loop.
        actor.fsm.ChangeState(TestActor.States.Turn);
        await UniTask.WaitUntil(() => actor.fsm.State == TestActor.States.Idle);
        AssertLog(actor.ObjectHistory, "DoneTurning");
        
        actor.fsm.ChangeState(TestActor.States.TestStateX);
        actor.fsm.ChangeState(TestActor.States.TestStateY);
        // state y should not execute, and state x will change the state in _Exit.
        AssertLog(actor.ObjectHistory, "TestStateX_Exit", "TestStateZ_Enter");

        

        try
        {
          actor.fsm.ChangeState(TestActor.States.Dead);
          await UniTask.Yield();
          Assert.Fail("Exception was not thrown.");
        }
        catch
        {
          
        }
        // clean up.
        actor.fsm.Dispose();

      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
        throw;
      }
    });
  }
}