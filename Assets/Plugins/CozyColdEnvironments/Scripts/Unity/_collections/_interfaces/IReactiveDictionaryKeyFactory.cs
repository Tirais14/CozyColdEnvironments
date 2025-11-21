using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public interface IReactiveDictionaryKeyFactory<TKey, TValue> : IReactiveDictionary<TKey, TValue>
    {
        Func<TKey> KeyFactory { get; set; }

        void Add(TValue value);
    }
}
