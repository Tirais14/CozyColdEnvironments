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
            ThrowIfDisposed();

            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            if (IsDone)
                throw new InvalidCastException("Already done");

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
