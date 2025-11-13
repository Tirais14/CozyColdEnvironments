using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Collections
{
    public interface ICCDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        Result<TValue> this[TKey key] { get; set; }

        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }

        void Add(TKey key, TValue value);

        bool Remove(TKey key);

        bool ContainsKey(TKey key);
    }
}
