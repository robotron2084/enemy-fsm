using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.enemyhideout.fsm.tests
{
  [TestFixture]
  public class TestFsm
  {
            
    public class TestActor
    {
      public enum States
      {
        Idle,
        Sprint,
        Running,
        Dead
      }

      public List<string> ObjectHistory = new List<string>();

      public EnemyFsm<States> fsm;
      
      public int _framesRunning = 0;

      public TestActor()
      {
        fsm = new EnemyFsm<States>(this);
      }
      
      public async Task Sprint_Enter(CancellationToken token)
      {
        Log("Entering Sprint...");
        await Task.Delay(1000);
        if (token.IsCancellationRequested)
        {
          Log("Exiting Sprint Early.");
          return;
        }
        Log("Sprint Entered!");
      }

      public async Task Running_Enter(CancellationToken token)
      {
        Log("Ready...");
        await Task.Delay(100);
        Log("Set...");
        await UniTask.Yield();
        Log("Go!!!");
      }

      public void Running_Update()
      {
        if (_framesRunning < 5)
        {
          Log($"Running {_framesRunning++}");
        }
      }
      
      public Task Running_Exit(CancellationToken token)
      {
        Log("Exiting Run...");
        Task.Delay(500);
        Log("Done running.");
        
        return Task.CompletedTask;
      }
      
      public Task Dead_Enter(CancellationToken token)
      {
        throw new Exception("I cannot die!");
      }

      void Log(string msg)
      {
        ObjectHistory.Add(msg);
      }
    }

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