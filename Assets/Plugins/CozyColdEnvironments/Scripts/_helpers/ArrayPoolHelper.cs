using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System.Buffers;

#nullable enable
namespace CCEnvs
{
    public static class ArrayPoolHelper
    {
        public static Pooled<T[]> RentHandled<T>(this ArrayPool<T> source, int minLength)
        {
            Guard.IsNotNull(source, nameof(source));

            T[] rented = source.Rent(minLength);
            var handle = new Pooled<T[]>(rented, (source, rented),
                static (input, arr) =>
                {
                    input.To<ArrayPool<T>>().Return(arr);
                });

            return handle;
        }
    }
}
