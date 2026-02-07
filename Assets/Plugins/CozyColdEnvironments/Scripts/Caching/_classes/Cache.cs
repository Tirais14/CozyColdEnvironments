#nullable enable
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Timers;
using UnityEditor;

#pragma warning disable S3267
namespace CCEnvs.Caching
{
    public sealed class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly Dictionary<TKey, ICacheEntry<TValue>> entries = new();
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

        public IEnumerable<TKey> Keys {
            get
            {
                return from entry in entries
                       where entry.Value.IsValid()
                       select entry.Key;
            }
        }

        public IEnumerable<TValue> Values {
            get
            {
                return from entry in entries.Values
                       where entry.IsValid()
                       select entry.GetValue();
            }
        }

        public Cache()
        {
            timer = new Timer
            {
                AutoReset = true
            };

            timer.Elapsed += ValidateEntries;

            ExpirationScanFrequency = 1.Minutes();
        }

        public ICacheEntry<TValue> CreateEntry(TKey key)
        {
            Guard.IsNotNull(key, nameof(key));

            if (entries.ContainsKey(key))
                throw new InvalidOperationException($"Cache entry with key: {key} already exists");

            var entry = new CacheEntry<TValue>(default);

            lock (lockObject)
            {
                entries.TryAdd(key, entry);
            }

            return entry;
        }

        public TValue? Get(TKey key)
        {
            Guard.IsNotNull(key, nameof(key));

            lock (lockObject)
            {
                if (!entries.TryGetValue(key, out var entry)
                    ||
                    !entry.IsValid())
                {
                    return default;
                }

                return entry.GetValue();
            }
        }

        public bool TryGet(TKey key, [NotNullWhen(true)] out TValue? result)
        {
            Guard.IsNotNull(key, nameof(key));

            result = Get(key);

            return result.IsNotDefault();
        }

        public TValue GetOrCreate(
            TKey key,
            Func<ICacheEntry<TValue>, TValue> factory)
        {
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(factory, nameof(factory));

            ICacheEntry<TValue> entry;

            lock (lockObject)
            {
                if (entries.TryGetValue(key, out entry))
                {
                    if (!entry.IsValid())
                        entry.SetValue(factory(entry));

                    return entry.GetValue()!;
                }
            }

            entry = CreateEntry(key);
            var value = factory(entry);
            entry.SetValue(value);

            return value;
        }

        public bool TryRemove(TKey? key, [NotNullWhen(true)] out TValue? value)
        {
            if (key is null)
            {
                value = default;
                return false;
            }

            ICacheEntry<TValue> entry;

            lock (lockObject)
            {
                if (!entries.TryGetValue(key, out entry)
                    ||
                    !entry.HasValue)
                {
                    value = default;
                    return false;
                }
            }

            value = entry.GetValue()!;
            entry.SetValue(default);

            return entry.IsValid();
        }

        public bool TryAdd(
            TKey key,
            TValue value, 
            [NotNullWhen(true)] out ICacheEntry<TValue>? entry)
        {
            Guard.IsNotNull(value, nameof(value));

            lock (lockObject)
            {
                if (entries.TryGetValue(key, out entry)
                    &&
                    entry.IsValid())
                {
                    entry.SetValue(value);
                    return true;
                }
            }

            entry = CreateEntry(key);
            entry.SetValue(value);

            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return entries.Where(static entry => entry.Value.IsValid())
                .Select(static entry => new KeyValuePair<TKey, TValue>(entry.Key, entry.Value.GetValue()!))
                .ToArray()
                .GetEnumeratorT();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
                    .ThenBy(entry =>
                    {
                        return Convert.ToByte(entry.Value.HasValue);
                    })
                    .Select(pair => pair.Key)
                    .Take(toRemoveCount)
                    .ToArray();

                foreach (var entryKey in toRemoveKeys)
                    entries.Remove(entryKey);
            }

            foreach (var pair in entries)
            {
                if (!pair.Value.HasValue)
                    continue;

                if (!pair.Value.IsExpired())
                    continue;

                pair.Value.SetValue(default);
            }
        }
    }
}
