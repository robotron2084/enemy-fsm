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

    public Func<CancellationToken, Task> OnEnterTask;
    public Func<CancellationToken, Task> OnExitTask;
    public Action OnEnter;
    public Action OnExit;
    public Action OnUpdate;

    public void Enter(Task exitingState)
    {
      if (OnEnterTask != null)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        Task_Enter(exitingState);
      }
      else
      {
        OnEnter?.Invoke();
        Update();
      }
    }

    public async void Update()
    {
      await Update(_cancellationTokenSource);
    }

    public async void Task_Enter(Task exitingState)
    {
      await exitingState;
      CancellationTokenSource cts = _cancellationTokenSource;
      await OnEnterTask(cts.Token);
      await Update(cts);
    }

    private async Task Update(CancellationTokenSource cts)
    {
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

    public async Task Exit()
    {
      _cancellationTokenSource.Cancel();
      if (OnExitTask != null)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        await OnExitTask(_cancellationTokenSource.Token);
      }
      else
      {
        OnExit?.Invoke();
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