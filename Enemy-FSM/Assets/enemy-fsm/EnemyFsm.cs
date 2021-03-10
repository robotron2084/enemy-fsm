using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.enemyhideout.fsm
{
  public class EnemyFsm<T> where T : Enum
  {
    public FsmState<T> CurrentState;

    public Dictionary<T, FsmState<T>> AllStates;
    
    public EnemyFsm()
    {
      AllStates = FsmCore<T>.GenerateStates();
      CurrentState = AllStates[default(T)];
    }

    public void ChangeState(T newState)
    {
      CurrentState =  FsmCore<T>.ChangeState(newState, CurrentState, AllStates);
    }

  }
}
