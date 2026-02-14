using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public class HashSetPool<T> : MutableCollectionPool<HashSet<T>, T>
    {
        public static HashSetPool<T> Shared { get; } = new(capacity: 1);

        public HashSetPool(
            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity: capacity,
                maxSize: maxSize)
        {

        }

#if UNITY_2017_1_OR_NEWER
        public override PooledObject<HashSet<T>> Get()
        {
            var listHandle = UnityEngine.Pool.HashSetPool<T>.Get(out var list);

            return PooledObject.Create(list, listHandle,
                static (_, handle) =>
                {
                    ((IDisposable)handle).Dispose();
                });
        }

        public override void Return(HashSet<T>? obj)
        {
            UnityEngine.Pool.HashSetPool<T>.Release(obj);
        }
#endif
    }
}
