using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Caching
{
    public interface IReferenceCache<TKey, TValue> where TValue : class
    {
        TimeSpan ExpirationScanFrequency { get; set; }
        int? SizeLimit { get; set; }

        Maybe<TValue> Get(TKey key);

        Maybe<TValue> GetOrCreate(TKey key, Func<IReferenceCacheEntry<TValue>, TValue> factory);

        bool TryAdd(TKey key, TValue value, [NotNullWhen(true)] out IReferenceCacheEntry<TValue>? entry);

        bool Remove(TKey? key);

        IReferenceCacheEntry<TValue> CreateEntry(TKey key);
    }
}
