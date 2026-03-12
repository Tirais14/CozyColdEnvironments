#nullable enable
using System;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public abstract partial class Command : CommandBase<ICommand>, ICommand
    {
        public static CommandBuilder Builder { get; } = CommandBuilder.Create();

        protected Command(bool isResetable = false)
            :
            base(isResetable: isResetable)
        {
        }

        public virtual void Execute()
        {
            if (CancellationToken.IsCancellationRequested)
            {
                SetCanceled();
                return;
            }

            if (!IsValid)
            {
                PrintIsNotValidError();
                return;
            }

            if (IsRunning)
            {
                PrintAlreadyExecutedError();
                return;
            }

            if (IsDone)
            {
                PrintAlreadyDoneError();
                return;
            }

            isExecuted = true;

            try
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    SetCanceled();
                    return;
                }

                OnExecute();

                SetCompleted();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case TaskCanceledException or OperationCanceledException:
                        SetCanceled();
                        this.PrintExceptionAsLog(ex);
                        break;
                    default:
                        SetFaulted(ex);
                        break;
                }
            }
            finally
            {
                //Prevents the callback triggering after execution completed
                //TrySetDefaultCancellationToken();

                DetachCancellationTokens();
            }
        }

        protected abstract void OnExecute();
    }
}
