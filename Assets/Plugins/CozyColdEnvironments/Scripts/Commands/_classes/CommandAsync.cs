#nullable enable
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
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
            ThrowIfDisposed();

            Guard.IsFalse(IsRunning, nameof(IsRunning), "Is already running");
            Guard.IsFalse(IsDone, nameof(IsDone), "Already done");

            isExecuted = true;

            AttachExternalCancellationToken(cancellationToken);

            try
            {
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
                //Prevents the callback triggering after execution completed
                SetDefaultCancellationToken();
            }
        }

        protected abstract ValueTask OnExecuteAsync(CancellationToken cancellationToken);
    }
}
