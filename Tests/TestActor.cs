using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.enemyhideout.fsm.tests
{
  public partial class TestFsm
  { 
    public class TestActor
    {
      public enum States
      {
        Idle, // has no defined actions or tasks.
        Sprint, // only enter
        Running, // enterTask, update, exitTask
        Dead, // throws an exception (testing that task exceptions correctly bubble)
        Hiding, // update only
        DoubleJump, // enterTask + enterAction
        Turn, // enterAction, update, changes state in update.
        TestStateX, TestStateY, TestStateZ, // check that when changing state when exiting a state, that the state doesn't continue to the stale state. 
        
      }

      public List<string> ObjectHistory = new List<string>();

      public EnemyFsm<States> fsm;
      
      public int _framesRunning = 0;

      public TestActor()
      {
        fsm = new EnemyFsm<States>(this);
      }
      
      // test: only enter.
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

      // Test 3 phases enter/update/exit
      public async Task Running_Enter(CancellationToken token)
      {
        Log("Ready...");
        await Task.Delay(100);
        Log("Set...");
        await Task.Delay(100);
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

      // Test: update only.
      public void Hiding_Update()
      {
        Log("Hiding");
      }
      
      // Test: enter task AND action

      public void DoubleJump_Enter()
      {
        Log("DoubleJump");
      }

      public async Task DoubleJump_Enter(CancellationToken ct)
      {
        Log("DoubleJump2");
        await Task.Delay(100);
        Log("DoubleJump3");
      }

      private int turnIndex;
      public void Turn_Enter()
      {
        turnIndex = 0;
      }

      public void Turn_Update()
      {
        turnIndex++;
        if (turnIndex == 5)
        {
          Log("DoneTurning");
          fsm.ChangeState(States.Idle);
        }
      }

      public void TestStateX_Exit()
      {
        Log("TestStateX_Exit");
        fsm.ChangeState(States.TestStateZ);
      }

      public void TestStateY_Enter()
      {
        Log("TestStateY_Enter");
      }
      public void TestStateY_Update()
      {
        Log("TestStateY_Update");
      }

      public void TestStateZ_Enter()
      {
        Log("TestStateZ_Enter");
      }


      void Log(string msg)
      {
        ObjectHistory.Add(msg);
      }
    }
  }
}