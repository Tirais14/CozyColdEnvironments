using System;
using System.Collections.Generic;
using UniRx;
using Unity.XR.OpenVR;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;

#nullable enable
namespace UTIRLib.UI
{
    public abstract class ViewModel<T> : IViewModel, IDisposableContainer
    {
        private readonly DisposableCollection disposables = new();
        private bool disposedValue;

        protected T model;

        protected ViewModel(T model)
        {
            this.model = model;
        }

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
