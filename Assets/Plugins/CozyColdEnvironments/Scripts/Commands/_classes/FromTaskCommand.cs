#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class FromTaskCommand : PoolableCommandAsync
    {
        public Task? Task { get; set; }

        public override bool IsCancelled => Task?.IsCanceled ?? false;
        public override bool IsFaulted => Task?.IsFaulted ?? false;
        public override bool IsCompleted => Task?.IsCompletedSuccessfully ?? false;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (Task is null)
                return;

            await Task;
        }

        protected override void OnReset()
        {
            base.OnReset();
            Task = null;
        }
    }

    public sealed class FromTaskCommand<T> : PoolableCommandAsync
    {
        public Task? Task { get; set; }

        public override bool IsCancelled => Task?.IsCanceled ?? false;
        public override bool IsFaulted => Task?.IsFaulted ?? false;
        public override bool IsCompleted => Task?.IsCompletedSuccessfully ?? false;

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (Task is null)
                return;

            await Task;
        }

        protected override void OnReset()
        {
            base.OnReset();
            Task = null;
        }
    }
}
