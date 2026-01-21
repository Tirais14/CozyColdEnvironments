#nullable enable
using R3;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public partial class Command 
    {
        public static CommandBuilder Builder { get; } = CommandBuilder.Create();
    }

    public abstract partial class Command : ICommand, IEquatable<Command>
    {
        public static CompletedCommand Completed { get; } = new();

        private readonly ReactiveProperty<CommandStatus> status = new();
        private readonly Type type;

        private bool isExecuted = new();

        private CancellationTokenSource? cancellationTokenSource;

        public string Name { get; } = string.Empty;

        public virtual bool IsReadyToExecute => !IsRunning && !IsDone;
        public virtual bool IsCancelled => status.Value == CommandStatus.Canceled;
        public virtual bool IsFaulted => status.Value == CommandStatus.Faulted;
        public virtual bool IsCompleted => status.Value == CommandStatus.Completed;
        public virtual bool IsRunning => isExecuted && !IsDone;

        public bool IsDone => IsCompleted || IsCancelled || IsFaulted;
        public bool IsSingle { get; }
        public bool IsResetable { get; }

        public int DelayFrameCount { get; set; }

        public CommandStatus Status => status.Value;

        protected Command(
            bool isSingle = false,
            string? name = null,
            bool isResetable = true,
            int delayFrameCount = 0)
        {
            Name = name ?? GetType().ToString();
            IsSingle = isSingle;
            IsResetable = isResetable;
            DelayFrameCount = delayFrameCount;

            type = GetType();
        }

        public static bool operator ==(Command? left, Command right)
        {
            return ReferenceEquals(left, right)
                   ||
                   (left is not null && left.Equals(right));
        }

        public static bool operator !=(Command? left, Command right)
        {
            return !(left == right);
        }

        public async ValueTask ExecuteAsync()
        {
            if (IsRunning || IsDone)
                return;

            isExecuted = true;

            CancelAndDisposeCancellationTokenSource();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await OnExecuteAsync(cancellationTokenSource.Token);
                status.Value = CommandStatus.Completed;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case TaskCanceledException:
                        status.Value = CommandStatus.Canceled;
                        break;
                    default:
                        status.Value = CommandStatus.Faulted;
                        this.PrintException(ex);
                        break;
                }
            }

            if (!IsDone
                &&
                (cancellationTokenSource is null
                ||
                cancellationTokenSource.IsCancellationRequested))
            {
                status.Value = CommandStatus.Canceled;
            }

            CancelAndDisposeCancellationTokenSource();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Undo()
        {
            if (cancellationTokenSource is null)
                throw new InvalidOperationException($"{nameof(CancellationTokenSource)} not found");

            CancelAndDisposeCancellationTokenSource();

            OnUndo();

            if (!IsFaulted)
                status.Value = CommandStatus.Canceled;
        }

        public bool TryReset()
        {
            if (!IsResetable)
                return false;

            if (!IsDone && !IsRunning)
                return true;

            if (IsRunning)
                Undo();

            status.Value = CommandStatus.None;
            isExecuted = false;

            OnReset();

            return true;
        }

        public ICommand Reset()
        {
            if (!IsResetable)
                throw new InvalidOperationException($"Command: {this} is not resetable");

            TryReset();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(GetType(), Name);
        }

        public override string ToString()
        {
            return $"({Name}: {Name}; {IsDone}: {IsDone}; {IsReadyToExecute}: {IsReadyToExecute})";
        }

        public bool Equals(Command other)
        {
            return Name == other.Name
                   &&
                   type == other.type;
        }

        public override bool Equals(object obj)
        {
            return obj is Command typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, Name);
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            return status.Where(
                static status =>
                {
                    return status == CommandStatus.Completed
                           ||
                           status == CommandStatus.Canceled
                           ||
                           status == CommandStatus.Faulted;
                });
        }

        bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                CancelAndDisposeCancellationTokenSource();
                status.Dispose();
            }

            disposed = true;
        }

        protected abstract ValueTask OnExecuteAsync(CancellationToken cancellationToken);

        protected virtual void OnUndo()
        {
        }

        protected virtual void OnReset()
        {
        }

        private void CancelAndDisposeCancellationTokenSource()
        {
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }
    }
}
