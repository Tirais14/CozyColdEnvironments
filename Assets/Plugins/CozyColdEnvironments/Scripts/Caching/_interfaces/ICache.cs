using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Caching
{
    public interface ICache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        TimeSpan ExpirationScanFrequency { get; set; }
        int? SizeLimit { get; set; }

        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }

        Maybe<TValue> Get(TKey key);

        TValue GetOrCreate(TKey key, Func<ICacheEntry<TValue>, TValue> factory);

        bool TryAdd(TKey key, TValue value, [NotNullWhen(true)] out ICacheEntry<TValue>? entry);

        bool TryRemove(TKey? key, [NotNullWhen(true)] out TValue? value);

        ICacheEntry<TValue> CreateEntry(TKey key);
    }
}
