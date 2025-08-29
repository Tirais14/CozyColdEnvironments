using System;

#pragma warning disable S3881
#nullable enable
namespace CozyColdEnvironments.Disposables
{
    public class DisposableContainer : IDisposableContainer
    {
        private bool disposedValue;

        protected readonly DisposableCollection disposables = new();

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                disposables.Dispose();

            disposedValue = true;
        }
    }
}
