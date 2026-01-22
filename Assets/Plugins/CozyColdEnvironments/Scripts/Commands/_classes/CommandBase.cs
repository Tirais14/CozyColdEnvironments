#nullable enable
using R3;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public abstract partial class CommandBase : ICommandBase
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
            if (!IsResetable)
                return false;

            if (IsRunning)
                Undo();

            status.Value = CommandStatus.None;
            isExecuted = false;

            OnReset();

            return true;
        }

        public ICommandBase Reset()
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

        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void OnUndo()
        {
        }

        protected virtual void OnReset()
        {
        }
    }
}
