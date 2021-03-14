using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace com.enemyhideout.fsm
{
  public class EnemyFsm<T> where T : Enum
  {
    public FsmState<T> CurrentState;

    public Dictionary<T, FsmState<T>> AllStates;
    
    public EnemyFsm(object actor)
    {
      AllStates = FsmCore<T>.GenerateStates();
      FsmCore<T>.PopulateStates(AllStates, actor);
      ChangeState(default(T));
    }

    public void ChangeState(T newState)
    {
      Task exitingTask = null;
      if (CurrentState != null)
      {
        exitingTask = CurrentState.Exit();
      }
      
      CurrentState =  FsmCore<T>.ChangeState(newState, CurrentState, AllStates);
      CurrentState.Enter(exitingTask);
    }

    public void Dispose()
    {
      CurrentState?.Cancel();
    }

  }
}
