#nullable enable
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class CompletedCommandAsync : ICommandAsync, IEquatable<CompletedCommandAsync>
    {
        public bool IsReadyToExecute { get; } = true;
        public bool IsCancelled { get; }
        public bool IsSingle { get; } = false;
        public bool IsCompleted { get; } = true;
        public bool IsRunning { get; } = false;
        public bool IsDone { get; } = true;
        public bool IsFaulted { get; } = false;
        public bool IsResetable { get; } = false;
        public bool IsValid { get; } = true;
        public string Name { get; } = "Completed";

        public int DelayFrameCount {
            get => 0;
            set => _ = value;
        }

        public CommandStatus Status { get; } = CommandStatus.Completed;

        public Type CommandType { get; } = typeof(CompletedCommandAsync);

        public static bool operator ==(CompletedCommandAsync? left, CompletedCommandAsync? right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(CompletedCommandAsync? left, CompletedCommandAsync? right)
        {
            return !(left == right);
        }

        public ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return default;
        }

        public void Undo()
        {
        }

        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(typeof(CompletedCommandAsync), Name);
        }

        public override string ToString()
        {
            return $"({Name})";
        }

        public ICommandAsync Reset() => this;

        public bool TryReset() => false;

        public bool Equals(CompletedCommandAsync? other) => other != null;

        public override bool Equals(object obj)
        {
            return obj is CompletedCommandAsync typed && Equals(typed);
        }

        public override int GetHashCode() => 0;

        public void Dispose()
        {
        }

        public void Cancel()
        {
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            return Observable.Return(CommandStatus.Completed);
        }
    }
}
