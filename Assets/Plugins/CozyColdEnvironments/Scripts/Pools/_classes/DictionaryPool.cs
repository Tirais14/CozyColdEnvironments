using System;
using System.Collections.Generic;
using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Pools
{
    public class DictionaryPool<TKey, TValue> : MutableCollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
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
        public override PooledObject<Dictionary<TKey, TValue>> Get()
        {
            var uHandle = UnityEngine.Pool.DictionaryPool<TKey, TValue>.Get(out var dictionary);

            return new PooledObject<Dictionary<TKey, TValue>>(dictionary, uHandle,
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
