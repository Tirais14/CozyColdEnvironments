#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public abstract partial class CommandAsync : CommandBase<ICommandAsync>, ICommandAsync
    {
        protected CancellationTokenSource? cancellationTokenSource;

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

            if (IsRunning || IsDone)
                return;

            isExecuted = true;

            CancelAndDisposeCancellationTokenSource();
            cancellationTokenSource = new CancellationTokenSource();
            var linkedTokenSource = cancellationTokenSource.Token.LinkTokens(cancellationToken);

            try
            {
                await OnExecuteAsync(linkedTokenSource.Token);

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
            finally
            {
                linkedTokenSource.Dispose();
            }

            if (!IsCompleted
                &&
                (cancellationTokenSource is null
                ||
                cancellationTokenSource.IsCancellationRequested))
            {
                SetCanceled();
            }

            DisposeCancellationToken();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Undo()
        {
            ValidateDisposed();

            if (cancellationTokenSource is null)
                throw new InvalidOperationException($"{nameof(CancellationTokenSource)} not found");

            CancelAndDisposeCancellationTokenSource();

            base.Undo();
        }

        public override void Cancel()
        {
            base.Cancel();

            if (IsDone)
                return;

            CancelAndDisposeCancellationTokenSource();
        }

        protected void DisposeCancellationToken()
        {
            if (cancellationTokenSource is null)
                return;

            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        protected void CancelAndDisposeCancellationTokenSource()
        {
            if (cancellationTokenSource is null)
                return;

            cancellationTokenSource.Cancel();
            DisposeCancellationToken();
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposed)
                return;

            if (disposing)
            {
                CancelAndDisposeCancellationTokenSource();
            }

            disposed = true;
        }

        protected abstract ValueTask OnExecuteAsync(CancellationToken cancellationToken);
    }
}
