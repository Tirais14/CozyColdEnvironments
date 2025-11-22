using CCEnvs.Unity.Items;
using System.Collections.Generic;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public interface IReactiveDictionaryExtended<TKey, TValue> : IReactiveDictionary<TKey, TValue>
    {
        T[] AddCount<T>(int count) where T : TValue, new();

        TValue[] SetCount<T>(int count) where T : TValue, new();

        TValue[] RemoveCount(int count);

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(key, value);
    }
}
