using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System;
using System.Buffers;

#nullable enable
namespace CCEnvs
{
    public static class ArrayPoolHelper
    {
        public static PooledObject<T[]> RentHandled<T>(this ArrayPool<T> source, int minLength)
        {
            Guard.IsNotNull(source, nameof(source));

            T[] rented = source.Rent(minLength);

            var handle = new PooledObject<T[]>(rented, source,
                static (arr, args) =>
                {
                    args.To<ArrayPool<T>>().Return(arr);
                });

            return handle;
        }

        public static PooledArray<T> Get<T>(
            this ArrayPool<T> source,
            int count,
            int offset = 0)
        {
            Guard.IsNotNull(source, nameof(source));

            var handle = source.RentHandled(count);
            var segmentHandle = new PooledArray<T>(handle, count, offset);

            return segmentHandle;
        }
    }
}
