# Enemy FSM

Attempting to build my own generic FSM setup with the following criteria:
  * Unity Support
  * Unity Editor Support
  * Use either `async Task` or standard functions.
  * Based on Functional Core, Imperative Shell.
  * Using reflection to allow for lightweight setup.

Not looking for
  * Nested structures.
  * Triggers

Future Goals
  * Performance.
  * Unitask support.
  * Make it a package.

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
States aren't that helpful unless we have functions that react to them. You can declare functions in your MyActor class that match the names of the states in your enumeration:
```csharp
public class MyActor
{
 // Entering and existing a state are tasks that can be cancelled when state changes occur.
 public Task Idle_Enter(CancellationToken ct)
 {
   // entering
   Task.Delay(1000);
   Debug.Log("Entered!");
 }
 
 public Task Idle_Exit(CancellationToken ct)
 {
   Task.Delay(1000);
   
   // if you don't want this to run after the state has been executed, check the token:
   if(ct.IsCancellationRequested)
   {
    return;
   }
   Debug.Log("Exited!");
 }
 
 // States can have an update function which is called every frame:
 public void Idle_Update()
 {
   Debug.Log("Executed every frame!");
 }
}
```
The state machine continues to run until disposed of:
```csharp
fsm.Dispose();
```


Early days, check out the code if you like! This is an experiment to see how well the above things mix. I have a feeling they might work well together. We'll see!






