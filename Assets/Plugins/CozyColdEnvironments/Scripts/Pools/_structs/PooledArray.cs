using CCEnvs.Collections;
using System;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IDisposable
    {
        private readonly Pooled<T[]> pooledInternal;

        public ArraySegment<T> Value { get; }

        public PooledArray(Pooled<T[]> pooled, int count, int offset = 0)
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
