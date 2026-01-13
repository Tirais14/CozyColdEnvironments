#nullable enable
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Timers;
using UnityEditor;

#pragma warning disable S3267
namespace CCEnvs.Caching
{
    public sealed class ReferenceCache<TKey, TValue> : IReferenceCache<TKey, TValue>
        where TValue : class
    {
        private readonly Dictionary<TKey, IReferenceCacheEntry<TValue>> entries = new();
        private readonly object lockObject = new();
        private readonly Timer timer = new();

        private TimeSpan _expirationScanFrequency;
        private int? _sizeLimit;

        public TimeSpan ExpirationScanFrequency {
            get => _expirationScanFrequency;
            set
            {
                _expirationScanFrequency = value;
                timer.Interval = _expirationScanFrequency.TotalMilliseconds;
            } 
        }
        public int? SizeLimit {
            get => _sizeLimit;
            set
            {
                if (value is null)
                {
                    _sizeLimit = value;
                    return;
                }

                _sizeLimit = Math.Abs(value.Value);
            }
        }

        public ReferenceCache()
        {
            timer = new Timer
            {
                AutoReset = true
            };

            timer.Elapsed += ValidateEntries;

            ExpirationScanFrequency = 60.Seconds();
        }

        public IReferenceCacheEntry<TValue> CreateEntry(TKey key)
        {
            Guard.IsNotNull(key, nameof(key));

            lock (lockObject)
            {
                if (entries.ContainsKey(key))
                    throw new InvalidOperationException($"Cache entry with key: {key} already exists");

                var entry = new ReferenceCacheEntry<TValue>(null);
                entries.TryAdd(key, entry);

                return entry;
            }
        }

        public Maybe<TValue> Get(TKey key)
        {
            Guard.IsNotNull(key, nameof(key));

            lock (lockObject)
            {
                if (!entries.TryGetValue(key, out var entry))
                    return Maybe<TValue>.None;

                return entry.GetValue();
            }
        }

        public Maybe<TValue> GetOrCreate(
            TKey key,
            Func<IReferenceCacheEntry<TValue>, TValue> factory)
        {
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(factory, nameof(factory));

            lock (lockObject)
            {
                if (entries.TryGetValue(key, out var entry))
                    return entry.GetValue();

                entry = CreateEntry(key);
                var value = factory(entry);
                entry.SetValue(value);

                return value;
            }
        }

        public bool Remove(TKey? key)
        {
            if (key is null)
                return false;

            lock (lockObject)
            {
                if (!entries.TryGetValue(key, out var entry))
                    return false;

                if (entry.IsExpired())
                    return false;

                entry.SetValue(null);
                return true;
            }
        }

        public bool TryAdd(
            TKey key,
            TValue value, 
            [NotNullWhen(true)] out IReferenceCacheEntry<TValue>? entry)
        {
            Guard.IsNotNull(value, nameof(value));

            lock (lockObject)
            {
                if (entries.TryGetValue(key, out entry))
                {
                    if (!entry.IsExpired())
                        return false;

                    entry.SetValue(value);
                    return true;
                }
            }

            entry = CreateEntry(key);
            entry.SetValue(value);

            return true;
        }

        private void ValidateEntries(object sender, ElapsedEventArgs args)
        {
            if (SizeLimit is not null
                &&
                SizeLimit > 0
                &&
                entries.Count > SizeLimit.Value)
            {
                var toRemoveCount = entries.Count - SizeLimit.Value;

                var toRemoveKeys = entries.OrderBy(entry =>
                    {
                        return entry.Value.IdleTime.Ticks;
                    })
                    .Select(pair => pair.Key)
                    .Take(toRemoveCount)
                    .ToArray();

                foreach (var entryKey in toRemoveKeys)
                    entries.Remove(entryKey);
            }

            foreach (var pair in entries)
            {
                if (pair.Value.IsExpired())
                    Remove(pair.Key);
            }
        }
    }
}
