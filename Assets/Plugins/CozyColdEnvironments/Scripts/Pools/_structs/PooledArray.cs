using CCEnvs.Collections;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IDisposable
    {
        private readonly PooledHandle<T[]> pooledInternal;
        private readonly ArraySegment<T> value;

        public readonly ArraySegment<T> Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
        }

        public readonly int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value.Count;
        }

        public T this[int index] {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => value[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this.value[index] = value;
        }

        public PooledArray(PooledHandle<T[]> pooled, int count, int offset = 0)
            :
            this()
        {
            pooledInternal = pooled;

            value = pooled.Value.GetArraySegment(count, offset);
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
