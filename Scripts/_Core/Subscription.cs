using System;

#nullable enable
namespace UTIRLib
{
    public class Subscription : IDisposable
    {
        private readonly Delegate method;
        private bool disposedValue;

        public Subscription(Delegate method)
        {
            this.method = method;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
