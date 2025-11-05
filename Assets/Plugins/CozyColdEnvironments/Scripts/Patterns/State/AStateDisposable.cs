using CCEnvs.Disposables;
using System;
using System.Collections.Generic;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Patterns.States
{
    public abstract class AStateDisposable<T> 
        :
        AState<T>, 
        IDisposable

        where T : IStateMachine
    {
        private readonly List<IDisposable> disposables = new();
        private bool disposedValue;

        protected AStateDisposable(T stateMachine) : base(stateMachine)
        {
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    disposables.DisposeAll();

                disposedValue = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
