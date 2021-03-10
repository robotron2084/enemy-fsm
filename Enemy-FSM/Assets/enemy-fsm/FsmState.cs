namespace com.enemyhideout.fsm
{
  public class FsmState<T>
  {
    public T State;
    
    public FsmState(T state)
    {
      State = state;
    }
  }
}