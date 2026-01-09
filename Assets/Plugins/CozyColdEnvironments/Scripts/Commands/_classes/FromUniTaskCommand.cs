#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class FromUniTaskCommand : Command
    {
        private readonly UniTask task;

        public override bool IsCancelled => task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => task.Status == UniTaskStatus.Faulted;

        public FromUniTaskCommand(UniTask task)
            :
            base(isSingle: false)
        {
            this.task = task;
        }

        protected override void OnExecute()
        {
        }
    }

    public class FromUniTaskCommand<T> : Command
    {
        private readonly UniTask<T> task;

        public override bool IsCancelled => task.Status == UniTaskStatus.Canceled;
        public override bool IsCompleted => task.Status == UniTaskStatus.Succeeded;
        public override bool IsFaulted => task.Status == UniTaskStatus.Faulted;

        public FromUniTaskCommand(UniTask<T> task)
            :
            base(isSingle: false)
        {
            this.task = task;
        }

        protected override void OnExecute()
        {
        }
    }
}
#endif