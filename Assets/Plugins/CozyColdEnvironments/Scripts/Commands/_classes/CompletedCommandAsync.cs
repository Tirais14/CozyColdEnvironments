#nullable enable
using CCEnvs.Reflection;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class CompletedCommandAsync
        :
        ICommandAsync,
        IEquatable<CompletedCommandAsync?>
    {
        public bool IsReadyToExecute => false;
        public bool IsCancelled => false;
        public bool IsSingle => false;
        public bool IsCompleted => true;
        public bool IsRunning => false;
        public bool IsDone => true;
        public bool IsFaulted => false;
        public bool IsResetable => false;
        public bool IsValid => true;
        public bool ExecuteOnThreadPool => false;

        public string Name => "Completed";

        public CommandStatus Status => CommandStatus.Completed;

        public Type CommandType => TypeofCache<CompletedCommandAsync>.Type;

        public CommandSignature Signature => new(TypeofCache<CompletedCommandAsync>.Type, Name, 545423464);

        public ICommandAsync AttachExternalCancellationToken(CancellationToken cancellationToken)
        {
            return this;
        }

        public void Cancel()
        {
        }

        public void Dispose()
        {
        }

        public bool Equals(CompletedCommandAsync? other)
        {
            return other is not null;
        }

        public override bool Equals(object obj)
        {
            return obj is CompletedCommandAsync typed && Equals(typed);
        }

        public ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return default;
        }

        public IDisposable GetCancellationHandle()
        {
            return Disposable.Empty;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            return Observable.Return(Status);
        }

        public Observable<CommandStatus> ObserveStatus()
        {
            return Observable.Return(Status);
        }

        public ICommandAsync Reset()
        {
            return this;
        }

        public bool TryReset()
        {
            return IsResetable;
        }

        public void Undo()
        {
        }

        public ValueTask WaitForDone(CancellationToken cancellationTokenAdditional = default)
        {
            return default;
        }
    }
}
