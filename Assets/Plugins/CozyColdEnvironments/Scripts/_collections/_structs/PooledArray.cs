#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CCEnvs
{
    public struct PooledArray<T> : IDisposable
    {
        private readonly ArrayPool<T> pool;

        public T[] RentedArray { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public readonly ArraySegment<T> Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(RentedArray, offset: 0, Length); }
        public bool IsDisposed { get; private set; }

        public PooledArray(ArrayPool<T> pool, IEnumerable<T> collection)
        {
            this.pool = pool;

            Length = collection.Count();
            RentedArray = pool.Rent(Length);

            int i = 0;
            foreach (var item in collection)
                RentedArray[i++] = item;

            IsDisposed = false;
        }

        public PooledArray(IEnumerable<T> collection)
            :
            this(ArrayPool<T>.Shared, collection)
        {
        }

        public static implicit operator bool(PooledArray<T> source)
        {
            return !source.IsDisposed;
        }

        public void Dispose()
        {
            pool.Return(RentedArray);
            RentedArray = null!;
            IsDisposed = true;
        }
    }
}
