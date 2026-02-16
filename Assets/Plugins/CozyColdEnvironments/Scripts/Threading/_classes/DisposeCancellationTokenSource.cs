using System.Threading;

namespace CCEnvs.Threading
{
    public class DisposeCancellationTokenSource : CancellationTokenSource
    {
        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                Cancel();

            disposed = true;

            base.Dispose(disposing);
        }
    }
}
