using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace com.enemyhideout.fsm
{
  public class EnemyFsm<T> where T : Enum
  {
    public T State {
      get { return _currentState.State; }
    }

    private FsmState<T> _currentState;

    public Dictionary<T, FsmState<T>> AllStates;
    
    public EnemyFsm(object actor)
    {
      AllStates = FsmCore<T>.GenerateStates();
      FsmCore<T>.PopulateStates(AllStates, actor);
      ChangeState(default(T));
    }

    public void ChangeState(T newState)
    {
      if (_currentState != null && newState.Equals(State))
      {
        return;
      }
      FsmState<T> nextState = FsmCore<T>.ChangeState(newState, _currentState, AllStates);
      FsmState<T> previousState = _currentState;
      _currentState = nextState;
      nextState.Enter(previousState);
    }

    public void Dispose()
    {
      _currentState?.Cancel();
    }

  }
}
