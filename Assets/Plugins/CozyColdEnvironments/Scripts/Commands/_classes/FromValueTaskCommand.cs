using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public class FromValueTaskCommand : CommandAsync
    {
        private readonly ValueTask task;

        public override bool IsCancelled => task.IsCanceled;
        public override bool IsCompleted => task.IsCompletedSuccessfully;
        public override bool IsFaulted => task.IsFaulted;

        public FromValueTaskCommand(ValueTask task)
            :
            base(isSingle: false)
        {
            this.task = task;
        }

        protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            return default;
        }
    }

    public class FromValueTaskCommand<T> : CommandAsync
    {
        private readonly ValueTask<T> task;

        public override bool IsCancelled => task.IsCanceled;
        public override bool IsCompleted => task.IsCompletedSuccessfully;
        public override bool IsFaulted => task.IsFaulted;

        public FromValueTaskCommand(ValueTask<T> task)
            :
            base(isSingle: false)
        {
            this.task = task;
        }

        protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            return default;
        }
    }
}
