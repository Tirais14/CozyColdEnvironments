#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class FromTaskCommand : Command
    {
        private readonly Task task;

        public override bool IsCancelled => task.IsCanceled;
        public override bool IsFaulted => task.IsFaulted;
        public override bool IsCompleted => task.IsCompletedSuccessfully;

        public FromTaskCommand(Task task)
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
