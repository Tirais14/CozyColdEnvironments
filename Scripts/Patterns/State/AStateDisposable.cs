using UTIRLib.Disposables;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Patterns.States
{
    public abstract class AStateDisposable<T> 
        :
        AState<T>,
        IDisposableContainer

        where T : IStateMachine
    {
        private readonly DisposableCollection disposables = new();
        private bool disposedValue;

        protected AStateDisposable(T stateMachine) : base(stateMachine)
        {
        }

        protected virtual void DisposaManaged() => disposables.Dispose();

        protected virtual void DisposeUnmanaged()
        {
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    DisposaManaged();

                DisposeUnmanaged();
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
