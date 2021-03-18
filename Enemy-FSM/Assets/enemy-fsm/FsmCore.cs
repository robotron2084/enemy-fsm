using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace com.enemyhideout.fsm
{
  public static class FsmCore<T>
  {
    private const string InvalidMethodSignatureMessage =
      "The signature for method {0} does not match the expected formats of Action or Func<CancellationToken,Task>.";
    
    private const BindingFlags methodFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
    BindingFlags.NonPublic;
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
      SetDelegateFor(state.State.ToString() + "_Enter", actor, ref state.OnEnterTask, ref state.OnEnter);
      SetDelegateFor(state.State.ToString() + "_Exit", actor, ref state.OnExitTask, ref state.OnExit);
      SetDelegateFor(state.State.ToString() + "_Update", actor, ref state.OnUpdate);
    }

    public static void SetDelegateFor(string methodName, object actor, ref Func<CancellationToken, Task> myTask,
      ref Action myAction)
    {

      if (SetDelegateFor(methodName, actor, ref myTask))
      {
        if (myTask == null)
        {
          if (!SetDelegateFor(methodName, actor, ref myAction))
          {
            // An error occurred!
            throw new InvalidCastException(string.Format(InvalidMethodSignatureMessage, methodName));
          }
          else
          {
            //either it was set...or nothing bad happened...
          }
        }
      }
      else
      {
        // an error occurred...but maybe it wasn't an error and this sig will work?! 
        if (!SetDelegateFor(methodName, actor, ref myAction))
        {
          // An error occurred!
          throw new InvalidCastException(string.Format(InvalidMethodSignatureMessage, methodName));
        }
      }
    }

    public static bool SetDelegateFor<T1, T2>(string methodName, object actor, ref Func<T1,T2> myTask)
    {
      Type t = actor.GetType();
      MethodInfo method = t.GetMethod(methodName, methodFlags);
      if (method != null)
      {
        if (method.ReturnType == typeof(T2))
        {
          var parameters = method.GetParameters();
          if (parameters.Length == 1)
          {
            myTask = CreateDelegate<Func<T1,T2>>(method, actor);
            return true;
          }
          else
          {
            return false;
          }
        }
        else
        {
          return false;
        }
      }

      return true; // no errors found.

    }

    public static bool SetDelegateFor(string methodName, object actor, ref Action myTask)
    {
      Type t = actor.GetType();
      MethodInfo method = t.GetMethod(methodName, methodFlags);
      if (method != null)
      {
        if (method.ReturnType == typeof(void))
        {
          if (method.GetParameters().Length == 0)
          {
            myTask = CreateDelegate<Action>(method, actor);
            return true;
          }
          else
          {
            return false;
          }
        }
        else
        {
          return false;
        }
      }
      return true;
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