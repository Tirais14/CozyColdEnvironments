using UniRx;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public interface IReactiveDictionaryExtended<TKey, TValue> : IReactiveDictionary<TKey, TValue>
    {
        T[] AddCount<T>(int count) where T : TValue, new();

        TValue[] SetCount<T>(int count) where T : TValue, new();

        TValue[] RemoveCount(int count);
    }
}
