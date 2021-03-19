using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace com.enemyhideout.fsm.tests
{
  public partial class TestFsm
  {
    public class TestActor
    {
      public enum States
      {
        Idle,
        Sprint,
        Running,
        Dead,
        Hiding,
        DoubleJump
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
        await UniTask.Yield();
        Log("DoubleJump3");
      }

      void Log(string msg)
      {
        ObjectHistory.Add(msg);
      }
    }
  }
}