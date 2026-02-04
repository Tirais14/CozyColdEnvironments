#nullable enable
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
            bool isSingle = false,
            string? name = null,
            bool isResetable = true,
            int delayFrameCount = 0)
            :
            base(isSingle: isSingle,
                name: name,
                isResetable: isResetable,
                delayFrameCount: delayFrameCount)
        {
        }

        public virtual async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            ValidateDisposed();

            Guard.IsFalse(IsRunning, nameof(IsRunning), "Is already running");
            Guard.IsFalse(IsDone, nameof(IsDone), "Already done");

            isExecuted = true;

            if (cancellationToken.CanBeCanceled)
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
                    case TaskCanceledException:
                        SetCanceled();
                        break;
                    case OperationCanceledException:
                        SetCanceled();
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
