#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static CCEnvs.Disposables.Subscription;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public partial class CommandAsync 
    {
        public static CommandBuilder Builder { get; } = CommandBuilder.Create();
    }

    public abstract partial class CommandAsync : CommandBase, ICommandAsync
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

        public async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (IsRunning || IsDone)
                return;

            isExecuted = true;

            CancelAndDisposeCancellationTokenSource();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                using var linkedTokenSource = cancellationTokenSource.Token.LinkTokens(cancellationToken);

                await OnExecuteAsync(linkedTokenSource.Token);
                status.Value = CommandStatus.Completed;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case TaskCanceledException:
                        status.Value = CommandStatus.Canceled;
                        break;
                    default:
                        status.Value = CommandStatus.Faulted;
                        this.PrintException(ex);
                        break;
                }
            }

            if (!IsCompleted
                &&
                (cancellationTokenSource is null
                ||
                cancellationTokenSource.IsCancellationRequested))
            {
                status.Value = CommandStatus.Canceled;
            }

            CancelAndDisposeCancellationTokenSource();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Undo()
        {
            if (cancellationTokenSource is null)
                throw new InvalidOperationException($"{nameof(CancellationTokenSource)} not found");

            CancelAndDisposeCancellationTokenSource();

            OnUndo();

            if (!IsFaulted)
                status.Value = CommandStatus.Canceled;
        }

        protected void CancelAndDisposeCancellationTokenSource()
        {
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
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
                status.Dispose();
            }

            disposed = true;
        }

        protected abstract ValueTask OnExecuteAsync(CancellationToken cancellationToken);
    }
}
