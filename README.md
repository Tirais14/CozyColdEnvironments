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
