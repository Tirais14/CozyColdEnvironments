using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable

#pragma warning disable S3881
namespace CCEnvs.Disposables
{
    public class DisposableCollection 
        : Collection<IDisposable>,
        IDisposableCollection
    {
        private bool disposed;

        public DisposableCollection()
        {
        }

        public DisposableCollection(IEnumerable<IDisposable> list) : base(list.ToArray())
        {
        }

        /// <summary>
        /// Disposes each element and clears the collection
        /// </summary>
        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                int disposableCount = Items.Count;
                for (int i = 0; i < disposableCount; i++)
                    Items[i].Dispose();

                Clear();
            }

            disposed = true;
        }
    }
}