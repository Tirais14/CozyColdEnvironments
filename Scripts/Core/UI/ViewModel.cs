using System;
using System.Collections.Generic;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI
{
    public abstract class ViewModel : IViewModel
    {
        private readonly List<IDisposable> disposables = new();
        private bool disposedValue;

        /// <exception cref="ArgumentNullException"></exception>
        public void BindDisposable(IDisposable disposable)
        {
            if (disposable.IsNull())
                throw new ArgumentNullException(nameof(disposable));

            disposables.Add(disposable);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < disposables.Count; i++)
                        disposables[i].Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
