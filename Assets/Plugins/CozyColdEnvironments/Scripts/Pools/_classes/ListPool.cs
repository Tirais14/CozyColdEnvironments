#nullable enable
using CCEnvs.Patterns.Factories;
using System;
using System.Collections.Generic;

namespace CCEnvs.Pools
{
    public class ListPool<T> : MutableCollectionPool<List<T>, T>
    {
        public static ListPool<T> Shared { get; } = new();

        public ListPool(
            int capacity = 4,
            int? maxSize = null)
            :
            base(factory: Factory.Create(static () => new List<T>()),
                capacity: capacity,
                maxSize: maxSize)
        {
            
        }

#if UNITY_2017_1_OR_NEWER
        public override PooledHandle<List<T>> Get()
        {
            var uHandle = UnityEngine.Pool.ListPool<T>.Get(out var list);

            return new PooledHandle<List<T>>(list, uHandle,
                static (list, uHandle) =>
                {
                    ((IDisposable)uHandle).Dispose();
                });
        }

        public override void Return(List<T>? obj)
        {
            UnityEngine.Pool.ListPool<T>.Release(obj);
        }
#endif
    }
}
