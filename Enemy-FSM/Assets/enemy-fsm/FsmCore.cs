using System;
using System.Collections.Generic;

namespace com.enemyhideout.fsm
{
  public static class FsmCore<T>
  {
    public static Dictionary<T, FsmState<T>> GenerateStates()
    {
      // this should get cached.
      Dictionary<T, FsmState<T>> retVal = new Dictionary<T, FsmState<T>>();
      
      var values = Enum.GetValues(typeof(T));
      foreach (var value in values)
      {
        var state = new FsmState<T>((T)value);
        retVal[(T)value] = state;
      }
      return retVal;
    }

    public static FsmState<T> ChangeState(T newState, FsmState<T> currentState, Dictionary<T, FsmState<T>> allStates)
    {
        return allStates[newState];
    }
  }
}