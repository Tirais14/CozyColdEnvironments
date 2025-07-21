using System;
using UTIRLib.Disposables;

#pragma warning disable S3881
#nullable enable
namespace UTIRLib.UI
{
    public class ViewModelBase : IDisposableContainer
    {
        private readonly DisposableCollection disposables = new();
        private bool disposedValue;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeManaged()
        {
            disposables.Dispose();
        }

        protected virtual void DisposeOther() { }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    DisposeManaged();

                DisposeOther();

                disposedValue = true;
            }
        }
    }
}
