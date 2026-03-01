# CommandScheduler
A frame-based, thread-safe command execution scheduler with reactive state observation and automatic garbage collection.

Features
- Frame-based execution: Commands are processed on frame ticks via IFrameRunnerWorkItem. Integrates with any FrameProvider or runs manually via OnFrame()
- Thread-safe scheduling: Schedule() can be called from any thread. Internal state uses ConcurrentQueue, ConcurrentDictionary, and lock(SyncRoot) for safe concurrent access
- Signature-based command cancellation: Scheduling a new command with an existing CommandSignature automatically cancels pending duplicates, preventing redundant work
- Dual-mode garbage collection: Lazy cleanup on dequeue + optional periodic bulk cleanup. Tunable via GarbageCollectEveryFrame, GarbageThreshold, and GargabeCommandCountThreshold
- Reactive state observation: Monitor scheduling and lifecycle events via R3 observables: ObserveScheduleCommand(), ObserveIsRunningStarted(), ObserveEnabled(), etc
- Sync/async command support: Executes both ICommand.Execute() and ICommandAsync.ExecuteAsync() uniformly
- Lifecycle control: Explicit Enable(), Disable(), Reset(), and Dispose() methods for precise runtime management
- Configurable idle behavior: DelayFrameCountBeforeRunningFinished controls how long IsRunning stays true after the queue empties, useful for UI feedback or transition logic
- Diagnostic logging: Built-in debug output via CCDebug.Instance when enabled, with command names and lifecycle events.

Features
-Garbage Collection: has 2 modes - periodic and lazy. Periodic

## ⚠️ Important Notes
- A command must be valid (IsValid == true) and not completed (IsDone == false) at the time of Schedule().
- After execution or cancellation, commands are recycled via Utilizable.TryUtilizeOrDispose().
- QueueCommand is an internal pooled wrapper — do not use directly.
 -For debugging, enable CCDebug.Instance.IsEnabled — the scheduler will log key lifecycle events.

# Command
Implementation of the Command pattern with async/await support, cancellation, status observation, and lifecycle management

Features
- Status Machine - None → Running → Completed / Canceled / Faulted
- Cancellation - Built-in CancellationToken support with automatic linking
- Observability - Reactive status stream via R3.Observable<CommandStatus>
- Safety -ThrowIfDisposed(), Guard.* checks, protection against re-entry
- CRTP - Generic template CommandBase<TThis> for fluent interface

CommandStatus values
- None — Command not started or reset
- Completed — Successfully finished
- Canceled — Cancelled by user or token
- Faulted — Finished with error

API
- Name: Command identifier (default: type name)
- Status: Current state
- IsDone, IsRunning, IsCancelled: State flags
- CancellationToken: Cancellation token for execution
- Execute/ExecuteAsync: Entry point (overridden in derived classes)
- TryReset: Reset to initial state
- AttachExternalCancellationToken: Link external cancellation token if it CanBeCancelled

## ⚠️ Important Notes
- Single Execution: Re-calling Execute() while IsRunning == true throws InvalidOperationException.
- Dispose: Command implements IDisposable. All operations are forbidden after Dispose().
- IsResetable: Set in constructor. If false, Reset() throws an exception.
- Exceptions: OperationCanceledException and TaskCanceledException are treated as cancellation; all others result in Faulted status.
- Thread Safety: Status updates are atomic via ReactiveProperty, but OnExecute* logic must be thread-safe if used externally.

## Exapmles

```C#
public class PrintCommand : Command
{
    private readonly string _message;
    
    public PrintCommand(string message) => _message = message;
    
    protected override void OnExecute()
    {
        Console.WriteLine(_message);
    }
}
```
```C#
public class FetchCommand : CommandAsync
{
    protected override async ValueTask OnExecuteAsync(CancellationToken ct)
    {
        await Task.Delay(1000, ct);
    }
}
```
## Command.Builder
Command.Builder returns a structure designed for fluent runtime command construction. It provides strict type safety and supports the implementation of all core methods defined in the Command interface.

Execution Modes
The builder supports two distinct finalization strategies for the constructed command:
1. Pooled Instance

- Behavior: The command instance is retrieved from an internal object pool.
- Lifecycle: Upon completion of execution within the CommandScheduler, the instance is automatically returned to the pool for reuse.
- Use Case: Optimized for high-frequency scenarios to reduce garbage collection overhead.

2. New Instance

- Behavior: A fresh command instance is allocated.
- Lifecycle: Once execution is complete, the Dispose method is explicitly invoked on the command instance.
- Use Case: Suitable for commands requiring unique state isolation or non-poolable resources.

Key Features
- Fluent Interface: Enables readable, chainable command definition at runtime.
- Strict Typing: Ensures compile-time safety for command parameters and return types.
- Resource Management: Automatic handling of instance lifecycle based on the selected mode (pool return vs. disposal).

### Example
```C#
Command.Builder.WithName("Activate Item Cannon")
    .SetSingle()
    .WithoutState()
    .Asyncronously()
    .WithExecuteAction(
    static async (cancellationToken) =>
    {
        await UniTask.DelayFrame(
            120,
            cancellationToken: cancellationToken
            );

        GameObjectQuery.Scene.IncludeInactive()
            .Component<ItemCannon>()
            .Strict()
            .enabled = true;
    })
    .BuildPooled()
    .Value
    .AttachExternalCancellationToken(destroyCancellationToken)
    .ScheduleBy(G.LoadingCommands);
```
