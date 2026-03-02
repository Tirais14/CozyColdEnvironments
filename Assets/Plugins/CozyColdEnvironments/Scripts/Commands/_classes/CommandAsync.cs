#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public abstract partial class CommandAsync : CommandBase<ICommandAsync>, ICommandAsync
    {
        protected CommandAsync(
            bool isResetable = true)
            :
            base(isResetable: isResetable)
        {
        }

        public virtual async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                SetCanceled();
                return;
            }

            ThrowIfDisposed();

            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            if (IsDone)
                throw new InvalidCastException("Already done");

            isExecuted = true;

            try
            {
                AttachExternalCancellationToken(cancellationToken);

                if (CancellationToken.IsCancellationRequested)
                {
                    SetCanceled();
                    return;
                }

                await OnExecuteAsync(CancellationToken);

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
                DetachCancellationTokens();
            }
        }

        protected abstract ValueTask OnExecuteAsync(CancellationToken cancellationToken);
    }
}
