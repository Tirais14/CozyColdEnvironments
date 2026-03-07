using System.Collections.Generic;
using ObservableCollections;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public interface IReactiveDictionaryExtended<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
        T[] AddCount<T>(int count) where T : TValue, new();

        TValue[] SetCount<T>(int count) where T : TValue, new();

        TValue[] RemoveCount(int count);

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(key, value);
    }
}
