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
  }
}