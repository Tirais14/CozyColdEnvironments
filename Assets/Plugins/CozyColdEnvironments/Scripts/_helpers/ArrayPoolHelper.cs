using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System.Buffers;

#nullable enable
namespace CCEnvs
{
    public static class ArrayPoolHelper
    {
        public static PooledHandle<T[]> RentHandled<T>(this ArrayPool<T> source, int minLength)
        {
            Guard.IsNotNull(source, nameof(source));

            T[] rented = source.Rent(minLength);

            var handle = new PooledHandle<T[]>(rented, source,
                static (arr, args) =>
                {
                    args.To<ArrayPool<T>>().Return(arr);
                });

            return handle;
        }

        public static PooledArray<T> RentHandled<T>(
            this ArrayPool<T> source,
            int minLength,
            int count,
            int offset = 0)
        {
            Guard.IsNotNull(source, nameof(source));

            var handle = source.RentHandled(minLength);
            var segmentHandle = new PooledArray<T>(handle, count, offset);

            return segmentHandle;
        }
    }
}
