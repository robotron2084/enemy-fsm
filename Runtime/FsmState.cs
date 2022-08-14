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

    private Task _exitTask;

    public void Enter(FsmState<T> previousState)
    {
      CancellationToken ct = _cancellationTokenSource.Token;
      // first we exit the previous state.
      previousState?.Exit();
      // user is exiting state X to then enter Y.
      // user changes state to Z via changestate in Exit
      // Enter() is called on new state Z calling Exit() on state Y
      // this means that cancellation should have been requested on Y
      // we should not enter Y at all.
      if (ct.IsCancellationRequested)
      {
        return;
      }
      
      OnEnter?.Invoke();
      if (OnEnterTask != null)
      {
        Task_Enter();
      }
      else
      {
        Update();
      }
    }

    public async void Update()
    {
      await Task_Update(_cancellationTokenSource.Token);
    }

    public async void Task_Enter()
    {
      CancellationTokenSource cts = _cancellationTokenSource;
      await OnEnterTask(cts.Token);
      await Task_Update(cts.Token);
    }

    private async Task Task_Update(CancellationToken ct)
    {
      if (!ct.IsCancellationRequested)
      {
        if (OnUpdate != null)
        {
          await Task_UpdateLoop(ct);
        } 
      }
    }

    private async Task Task_UpdateLoop(CancellationToken ct)
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
      if (_exitTask == null)
      {
        _exitTask = Task_Exit();
        await _exitTask;
        _exitTask = null;
      }
      // else: ChangeState was called from within _Exit().
    }

    public async Task Task_Exit()
    {
      OnExit?.Invoke();
      _cancellationTokenSource.Cancel(); // make sure nothing continues here.
      _cancellationTokenSource = new CancellationTokenSource(); // make a new cancellation token.
      if (OnExitTask != null)
      {
        await OnExitTask(_cancellationTokenSource.Token);
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