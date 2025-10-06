using CCEnvs.Disposables;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Patterns.States
{
    public abstract class AStateDisposable<T> 
        :
        AState<T>,
        IDisposableContainer

        where T : IStateMachine
    {
        private readonly Disposables.Disposables disposables = new();
        private bool disposedValue;

        protected AStateDisposable(T stateMachine) : base(stateMachine)
        {
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    disposables.Dispose();

                disposedValue = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
