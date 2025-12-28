using CCEnvs.Collections;
using System;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IDisposable
    {
        private readonly PooledHandle<T[]> pooledInternal;

        public IList<T> Value { get; }

        public PooledArray(PooledHandle<T[]> pooled, int count, int offset = 0)
            :
            this()
        {
            pooledInternal = pooled;
            Value = pooled.Value.GetArraySegment(count, offset);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            pooledInternal.Dispose();

            disposed = true;
        }
    }
}
