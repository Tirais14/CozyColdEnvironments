#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Threading.Tasks;

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
            ValidateDisposed();

            Guard.IsFalse(IsRunning, nameof(IsRunning), "Is already running");
            Guard.IsFalse(IsDone, nameof(IsDone), "Already done");

            isExecuted = true;

            try
            {
                OnExecute();

                SetCompleted();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case TaskCanceledException:
                        SetCanceled();
                        break;
                    default:
                        SetFaulted(ex);
                        break;
                }
            }

            //Prevents the callback triggering after execution completed
            SetDefaultCancellationToken();
        }

        protected abstract void OnExecute();
    }
}
