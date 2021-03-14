using System;
using System.Threading;
using System.Threading.Tasks;

namespace com.enemyhideout.fsm
{
  public class FsmState<T>
  {
    public T State;

    private CancellationTokenSource _cancellationTokenSource;
    public CancellationTokenSource CancellationTokenSource
    {
      get => _cancellationTokenSource;
      set => _cancellationTokenSource = value;
    }

    public Func<CancellationToken, Task> OnEnter;
    public Func<CancellationToken, Task> OnExit;
    public Action OnUpdate;

    public void Enter()
    {
      if (OnEnter != null)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        Task_Enter();
      }
    }

    public async void Task_Enter()
    {
      CancellationTokenSource cts = _cancellationTokenSource; //capture
      await OnEnter(_cancellationTokenSource.Token);
      if (!cts.Token.IsCancellationRequested)
      {
        if (OnUpdate != null)
        {
          await Task_Update(_cancellationTokenSource.Token);
        } 
      }
    }

    private async Task Task_Update(CancellationToken ct)
    {
      while (!ct.IsCancellationRequested)
      {
        await Task.Delay(1); // this should probably do some kind of yield on a sync context or something less hacky.
        if (!ct.IsCancellationRequested)
        {
          OnUpdate();
        }
      }
    }
    public async void Exit()
    {
      _cancellationTokenSource.Cancel();
      if (OnExit != null)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        await OnExit(_cancellationTokenSource.Token);
      }
    }

    public void Cancel()
    {
      _cancellationTokenSource.Cancel();
    }
    
    public FsmState(T state)
    {
      State = state;
      _cancellationTokenSource = new CancellationTokenSource();
    }
  }
}