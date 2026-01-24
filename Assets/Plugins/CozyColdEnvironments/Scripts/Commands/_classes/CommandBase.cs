#nullable enable
using R3;
using System;
using System.Runtime.CompilerServices;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public abstract partial class CommandBase<TThis> : ICommandBase
        where TThis : ICommandBase
    {

        protected readonly ReactiveProperty<CommandStatus> status = new();

        protected bool isExecuted;

        public string Name { get; } = string.Empty;

        public virtual bool IsReadyToExecute => !IsRunning && !IsDone;
        public virtual bool IsCancelled => status.Value == CommandStatus.Canceled;
        public virtual bool IsFaulted => status.Value == CommandStatus.Faulted;
        public virtual bool IsCompleted => status.Value == CommandStatus.Completed;
        public virtual bool IsRunning => isExecuted && !IsDone;

        public bool IsDone => IsCompleted || IsCancelled || IsFaulted;
        public bool IsSingle { get; }
        public bool IsResetable { get; }
        public bool IsValid => !disposed;

        public int DelayFrameCount { get; set; }

        public CommandStatus Status => status.Value;

        public Type CommandType { get; }

        protected CommandBase(
            bool isSingle = false,
            string? name = null,
            bool isResetable = true,
            int delayFrameCount = 0)
        {
            Name = name ?? GetType().ToString();
            IsSingle = isSingle;
            IsResetable = isResetable;
            DelayFrameCount = delayFrameCount;

            CommandType = GetType();
        }

        public abstract void Undo();

        public bool TryReset()
        {
            ValidateDisposed();

            if (!IsResetable)
                return false;

            try
            {
                OnReset();
            }
            finally
            {
                status.Value = CommandStatus.None;
                isExecuted = false;
            }

            return true;
        }

        public TThis Reset()
        {
            if (!IsResetable)
                throw new InvalidOperationException($"Command: {this} is not resetable");

            TryReset();

            return this.To<TThis>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(GetType(), Name);
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status})";
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            ValidateDisposed();

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

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            status.Dispose();

            disposed = true;
        }

        protected virtual void OnUndo()
        {
        }

        protected virtual void OnReset()
        {
        }

        protected void ValidateDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
        }
    }
}
