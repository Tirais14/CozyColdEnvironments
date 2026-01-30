using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public class FromValueTaskCommand : PoolableCommandAsync
    {
        public ValueTask Task { get; set; }

        public override bool IsCancelled => Task.IsCanceled;
        public override bool IsCompleted => Task.IsCompletedSuccessfully;
        public override bool IsFaulted => Task.IsFaulted;

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

    public class FromValueTaskCommand<T> : PoolableCommandAsync
    {
        public ValueTask<T> Task { get; set; }

        public override bool IsCancelled => Task.IsCanceled;
        public override bool IsCompleted => Task.IsCompletedSuccessfully;
        public override bool IsFaulted => Task.IsFaulted;

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
