using CCEnvs.Patterns.Factories;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public class DictionaryPool<TKey, TValue> : ObjectPool<Dictionary<TKey, TValue>>
    {
        public static DictionaryPool<TKey, TValue> Shared { get; } = new();

        public DictionaryPool(
            int capacity = 4,
            int? maxSize = null)
            :
            base(factory: Factory.Create(static () => new Dictionary<TKey, TValue>()),
                capacity: capacity,
                maxSize: maxSize)
        {
        }

#if UNITY_2017_1_OR_NEWER
        public override PooledHandle<Dictionary<TKey, TValue>> Get()
        {
            var uHandle = UnityEngine.Pool.DictionaryPool<TKey, TValue>.Get(out var dictionary);

            return new PooledHandle<Dictionary<TKey, TValue>>(dictionary, uHandle,
                static (list, uHandle) =>
                {
                    ((IDisposable)uHandle).Dispose();
                });
        }

        public override void Return(Dictionary<TKey, TValue>? obj)
        {
            UnityEngine.Pool.DictionaryPool<TKey, TValue>.Release(obj);
        }
#endif
    }
}
