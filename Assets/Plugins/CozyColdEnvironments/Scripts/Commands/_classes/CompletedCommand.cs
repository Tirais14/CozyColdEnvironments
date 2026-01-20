#nullable enable
using R3;
using System;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class CompletedCommand : ICommand, IEquatable<CompletedCommand>
    {
        public bool IsReadyToExecute { get; } = true;
        public bool IsCancelled { get; }
        public bool IsSingle { get; } = false;
        public bool IsCompleted { get; } = true;
        public bool IsRunning { get; } = false;
        public bool IsDone { get; } = true;
        public bool IsFaulted { get; } = false;
        public bool IsResetable { get; } = false;
        public string Name { get; } = "Completed";

        public int DelayFrameCount {
            get => 0;
            set => _ = value;
        }

        public CommandStatus Status { get; } = CommandStatus.Completed;

        public static bool operator ==(CompletedCommand? left, CompletedCommand? right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(CompletedCommand? left, CompletedCommand? right)
        {
            return !(left == right);
        }

        public ValueTask ExecuteAsync() => default;

        public void Undo()
        {
        }

        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(typeof(CompletedCommand), Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public ICommand Reset() => this;

        public bool TryReset() => false;

        public bool Equals(CompletedCommand? other) => other != null;

        public override bool Equals(object obj)
        {
            return obj is CompletedCommand typed && Equals(typed);
        }

        public override int GetHashCode() => 0;

        public void Dispose()
        {
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            return Observable.Empty<CommandStatus>();
        }
    }
}
