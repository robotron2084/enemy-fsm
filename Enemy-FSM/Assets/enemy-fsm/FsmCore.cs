using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace com.enemyhideout.fsm
{
  public static class FsmCore<T>
  {
    public static Dictionary<T, FsmState<T>> GenerateStates()
    {
      Dictionary<T, FsmState<T>> retVal = new Dictionary<T, FsmState<T>>();
      
      var values = Enum.GetValues(typeof(T));
      foreach (var value in values)
      {
        var state = new FsmState<T>((T)value);
        retVal[(T)value] = state;
      }
      return retVal;
    }

    public static void PopulateStates(Dictionary<T, FsmState<T>> allStates, object actor)
    {
      foreach (var kvp in allStates)
      {
        PopulateState(kvp.Value, actor);
      }
    }

    public static void PopulateState(FsmState<T> state, object actor)
    {
      SetDelegateFor(state.State.ToString() + "_Enter", actor, ref state.OnEnter);
      SetDelegateFor(state.State.ToString() + "_Exit", actor, ref state.OnExit);
      SetDelegateFor(state.State.ToString() + "_Update", actor, ref state.OnUpdate);
    }

    public static void SetDelegateFor<T1, T2>(string methodName, object actor, ref Func<T1,T2> myTask)
    {
      Type t = actor.GetType();
      MethodInfo method = t.GetMethod(methodName);
      if (method != null)
      {
        myTask = CreateDelegate<Func<T1,T2>>(method, actor);
      }
    }

    public static void SetDelegateFor(string methodName, object actor, ref Action myTask)
    {
      Type t = actor.GetType();
      MethodInfo method = t.GetMethod(methodName);
      if (method != null)
      {
        myTask = CreateDelegate<Action>(method, actor);
      }
    }

    public static FsmState<T> ChangeState(T newState, FsmState<T> currentState, Dictionary<T, FsmState<T>> allStates)
    {
        return allStates[newState];
    }

    private static V CreateDelegate<V>(MethodInfo method, Object target) where V : class
    {
      var ret = (Delegate.CreateDelegate(typeof(V), target, method) as V);

      if (ret == null)
      {
        throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
      }
      return ret;

    }

  }
}