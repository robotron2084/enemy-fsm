namespace com.enemyhideout.fsm.tests
{
  public class ActorBase
  {
    protected virtual void Idle_Enter()
    {
      
    }

    protected void NonVirtualParentAction()
    {
    }

    // not to be extended on purpose
    protected virtual void VirtualParentAction()
    {
    }

  }

  public class ChildActor : ActorBase
  {
    protected override void Idle_Enter()
    {
      
    }
  }
}