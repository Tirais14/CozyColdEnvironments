using CCEnvs.Pools;
using System.Buffers;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs
{
    public static class CollectionHelper
    {
        public static PooledArray<T> ToArrayPooled<T>(this ICollection<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            var arrHandle = ArrayPool<T>.Shared.RentHandled(source.Count);
            source.CopyTo(arrHandle.Value, 0);

            return new PooledArray<T>(arrHandle, source.Count, offset: 0);
        }
    }
}
