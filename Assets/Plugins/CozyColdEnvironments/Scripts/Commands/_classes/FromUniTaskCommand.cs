#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class FromUniTaskCommand : PoolableCommandAsync
    {
        public UniTask Task { get; set; }

        public override bool IsCancelled => Task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => Task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => Task.Status == UniTaskStatus.Faulted;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task;
            }
            catch (System.Exception ex)
            {
                this.PrintExceptionAsLog(ex, Diagnostics.DebugArguments.Editor);
            }
        }

        protected override void OnReset()
        {
            base.OnReset();
            Task = default;
        }
    }

    public class FromUniTaskCommand<T> : PoolableCommandAsync
    {
        public UniTask<T> Task { get; set; }

        public override bool IsCancelled => Task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => Task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => Task.Status == UniTaskStatus.Faulted;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task;
            }
            catch (System.Exception ex)
            {
                this.PrintExceptionAsLog(ex, Diagnostics.DebugArguments.Editor);
            }
        }

        protected override void OnReset()
        {
            base.OnReset();
            Task = default;
        }
    }
}
#endif