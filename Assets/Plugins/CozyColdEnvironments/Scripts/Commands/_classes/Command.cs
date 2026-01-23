#nullable enable
using System;

namespace CCEnvs.Patterns.Commands
{
    public abstract partial class Command : CommandBase<ICommand>, ICommand
    {
        public static CommandBuilder Builder { get; } = CommandBuilder.Create();

        protected Command(
            bool isSingle = false,
            string? name = null,
            bool isResetable = false,
            int delayFrameCount = 0)
            :
            base(isSingle: isSingle,
                name: name,
                isResetable: isResetable,
                delayFrameCount: delayFrameCount)
        {
        }

        public virtual void Execute()
        {
            if (IsRunning || IsDone)
                return;

            isExecuted = true;

            try
            {
                OnExecute();
                status.Value = CommandStatus.Completed;
            }
            catch (Exception ex)
            {
                status.Value = CommandStatus.Faulted;
                this.PrintException(ex);
                return;
            }

            if (!IsCompleted && !IsFaulted)
                status.Value = CommandStatus.Canceled;
        }

        public override void Undo()
        {
            OnUndo();

            if (!IsFaulted)
                status.Value = CommandStatus.Canceled;
        }

        protected abstract void OnExecute();
    }
}
