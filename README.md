# Enemy FSM

Attempting to build my own generic FSM setup with the following criteria:
  * Unity Support
  * Unity Editor Support
  * Use either `async Task` or standard functions.
  * Using Functional Core/Imperative Shell principles.
  * Using reflection to allow for lightweight setup.
  * Not tied to MonoBehaviours.

Not looking for:
  * Nested structures.
  * Triggers

### Future Goals

  * Performance.
  * [UniTask](https://github.com/Cysharp/UniTask) support ( kind of implicitly supported!)
  * Make it a package.
  * A better name! :D

# Example Usage

An FSM is comprised of 3 components:
  * An enumeration of states used for the fsm.
  * An instance of the `EnemyFsm` class.
  * An actor upon which the executions occur.

Let's say we have a unit that will have the following states:
```csharp
public enum States
{
  Idle,
  Sprint,
  Running,
  Dead
}
```
We need an 'actor' which will respond to state changes:
```csharp
public class MyActor
{
}
```
And control the actor with an fsm:
```csharp
MyActor myActor = new MyActor();
EnemyFsm<States> fsm = new EnemyFsm<States>(myActor);
```
We can change the state of the actor by doing:
```csharp
fsm.ChangeState(States.Sprint);
```
## Responding to State Change
States aren't that helpful unless we have functions that react to them. You can declare functions in your MyActor class that match the names of the states in your enumeration. Each state has an `_Enter`, `_Exit` and `_Update` handler  you can register.
```csharp
public class MyActor
{
    
 // Entering and exiting a state can be represented as a Task, an Action, or both.
 public void Idle_Enter()
 {
     Debug.Log("This is called first");
 }
    
 public Task Idle_Enter(CancellationToken ct)
 {
     Debug.Log("This is called second");
   // entering
   Task.Delay(1000);
   Debug.Log("Entered!");
 }
 
 // States can have an update function which is called every frame:
 public void Idle_Update()
 {
   Debug.Log("Executed every frame!");
 }
        
 public Task Idle_Exit(CancellationToken ct)
 {
   Task.Delay(1000);
   
   // if the state changes, you can exit early, and respond to the exit and clean up if necessary.
   if(ct.IsCancellationRequested)
   {
    return;
   }
   Debug.Log("Exited!");
 }

}
```
The state machine continues to run until disposed of:
```csharp
fsm.Dispose();
```


Early days, check out the code if you like! This is an experiment to see how well the above things mix. I have a feeling they might work well together. We'll see!

### Simple Gun Example

```
using System.Threading;
using System.Threading.Tasks;
using com.enemyhideout.fsm;
using UnityEngine;

namespace Code
{
  public class Gun : MonoBehaviour
  {
    public enum States
    {
      Idle,
      Fire
    }

    private EnemyFsm<States> fsm;
    public float firingRate = 0.5f;
      
    void Start()
    {
      fsm = new EnemyFsm<States>(this);
    }

    void Idle_Update()
    {
      if (Input.GetMouseButton(0))
      {
        fsm.ChangeState(States.Fire);
      }
    }

    async Task Fire_Enter(CancellationToken token)
    {
      Debug.Log("Fire");
      await Task.Delay((int)(firingRate * 1000));
      fsm.ChangeState(States.Idle);
    }
  }
}
```


