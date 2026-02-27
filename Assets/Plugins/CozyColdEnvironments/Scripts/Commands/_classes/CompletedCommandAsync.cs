#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace CCEnvs.Patterns.Commands
{
    public sealed class CompletedCommandAsync : ICommandAsync, IEquatable<CompletedCommandAsync>
    {
        public bool IsReadyToExecute { get; } = true;
        public bool IsCancelled { get; }

        public bool IsSingle {
            get => false;
            set => _ = value;
        }

        public bool IsCompleted { get; } = true;
        public bool IsRunning { get; } = false;
        public bool IsDone { get; } = true;
        public bool IsFaulted { get; } = false;
        public bool IsResetable { get; } = false;
        public bool IsValid { get; } = true;

        public string Name {
            get => "Completed";
            set => _ = value;
        }

        public int DelayFrameCount {
            get => 0;
            set => _ = value;
        }

        public CommandStatus Status { get; } = CommandStatus.Completed;

        public Type CommandType { get; } = typeof(CompletedCommandAsync);

        public CancellationToken CancellationToken { get; } = default;

        public CommandSignature Signature { get; }

        public Identifier ID { get; } = "Completed";

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

        public CommandSignature GetCommandSignature()
        {
            return new CommandSignature(typeof(CompletedCommandAsync), Name);
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

        public IDisposable GetCancellationHandle()
        {
            return Disposable.Empty;
        }

        public ICommandAsync AttachExternalCancellationToken(CancellationToken cancellationToken)
        {
            return this;
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            return Observable.Return(CommandStatus.Completed);
        }

        public Observable<CommandStatus> ObserveStatus()
        {
            throw new NotImplementedException();
        }
    }
}
