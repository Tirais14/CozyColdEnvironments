#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public class FromAsyncLazyCommand : PoolableCommandAsync
    {
        public AsyncLazy? TaskLazy { get; set; }

        public override bool IsCancelled => TaskLazy?.Task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => TaskLazy?.Task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => TaskLazy?.Task.Status == UniTaskStatus.Faulted;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (TaskLazy is null)
                return;

            await TaskLazy;
        }

        protected override void OnReset()
        {
            base.OnReset();
            TaskLazy = null!;
        }
    }

    public class FromAsyncLazyCommand<T> : PoolableCommandAsync
    {
        public AsyncLazy<T>? TaskLazy { get; set; }

        public override bool IsCancelled => TaskLazy?.Task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => TaskLazy?.Task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => TaskLazy?.Task.Status == UniTaskStatus.Faulted;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (TaskLazy is null)
                return;

            await TaskLazy;
        }

        protected override void OnReset()
        {
            base.OnReset();
            TaskLazy = null!;
        }
    }
}
#endif