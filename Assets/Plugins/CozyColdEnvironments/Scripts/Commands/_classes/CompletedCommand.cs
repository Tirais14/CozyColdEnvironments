#nullable enable
using System;

namespace CCEnvs.Patterns.Commands
{
    internal sealed class CompletedCommand : ICommand, IEquatable<CompletedCommand>
    {
        public bool IsReadyToExecute { get; } = true;
        public bool IsCancelled { get; }
        public bool IsSingle { get; } = false;
        public bool IsCompleted { get; } = true;
        public bool IsRunning { get; } = false;
        public bool IsDone { get; } = true;
        public bool IsFaulted { get; } = false;
        public bool IsResetable { get; } = false;
        public string CommandName { get; } = "Completed";
        public int DelayFrameCount { get; } = 0;

        public static bool operator ==(CompletedCommand? left, CompletedCommand? right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(CompletedCommand? left, CompletedCommand? right)
        {
            return !(left == right);
        }

        public void Execute()
        {
        }

        public void Undo()
        {
        }

        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(typeof(CompletedCommand), CommandName);
        }

        public override string ToString()
        {
            return CommandName;
        }

        public ICommand Reset() => this;

        public bool TryReset() => false;

        public bool Equals(CompletedCommand? other) => other != null;

        public override bool Equals(object obj)
        {
            return obj is CompletedCommand typed && Equals(typed);
        }

        public override int GetHashCode() => 0;
    }
}
