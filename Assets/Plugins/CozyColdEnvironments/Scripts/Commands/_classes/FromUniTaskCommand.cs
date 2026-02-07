#if UNITASK_PLUGIN
using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class FromUniTaskCommand : PoolableCommandAsync
    {
        private UniTask task;

        public UniTask Task {
            get => task;
            set
            {
                task = value;
                //task.Forget(static ex => CCDebug.Instance.PrintException(ex));
            }
        }

        //public override bool IsCancelled => Task.Status == UniTaskStatus.Canceled;
        //public override bool IsCompleted => Task.Status == UniTaskStatus.Succeeded;
        //public override bool IsFaulted => Task.Status == UniTaskStatus.Faulted;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            await task;
        }

        protected override void OnReset()
        {
            base.OnReset();
            Task = default;
        }
    }

    public class FromUniTaskCommand<T> : PoolableCommandAsync
    {
        private UniTask<T> task;

        public UniTask<T> Task {
            get => task;
            set
            {
                task = value;
                task.Forget(static ex => CCDebug.Instance.PrintException(ex));
            }
        }

        public override bool IsCancelled => Task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => Task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => Task.Status == UniTaskStatus.Faulted;

        protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            return default;
        }

        protected override void OnReset()
        {
            base.OnReset();
            Task = default;
        }
    }
}
#endif