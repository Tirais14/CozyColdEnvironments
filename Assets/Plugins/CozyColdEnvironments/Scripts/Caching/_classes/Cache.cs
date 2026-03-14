#nullable enable
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using UnityEditor;

#pragma warning disable S3267
namespace CCEnvs.Caching
{
    public sealed class Cache<TKey, TValue>
        :
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IDisposable
    {
        private readonly ConcurrentDictionary<TKey, ICacheEntry<TValue>> entries = new();

        private Timer? timer;

        private TimeSpan _expirationScanFrequency;

        private int? _sizeLimit;

        public TimeSpan ExpirationScanFrequency {
            get => _expirationScanFrequency;
            set
            {
                _expirationScanFrequency = value;
                OnExpirationScaneFrequencyChanged(_expirationScanFrequency);
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

                _sizeLimit = Math.Max(0, value.Value);
            }
        }

        public IEnumerable<TKey> Keys {
            get
            {
                return from entry in entries
                       where entry.Value.IsValid
                       select entry.Key;
            }
        }

        public IEnumerable<TValue> Values {
            get
            {
                return from entry in entries.Values
                       where entry.IsValid
                       select entry.GetValue();
            }
        }

        public object SyncRoot { get; } = new();

        public Cache()
        {
            ExpirationScanFrequency = 10.Seconds();
        }

        ~Cache() => Dispose();

        public ICacheEntry<TValue> CreateEntry(TKey key)
        {
            Guard.IsNotNull(key, nameof(key));

            if (entries.ContainsKey(key))
                throw new InvalidOperationException($"Cache entry with key: {key} already exists");

            var entry = new CacheEntry<TValue>(default);

            lock (SyncRoot)
                entries[key] = entry;

            return entry;
        }

        public TValue? GetValue(TKey key)
        {
            Guard.IsNotNull(key, nameof(key));

            if (!entries.TryGetValue(key, out var entry)
                ||
                !entry.IsValid)
            {
                return default;
            }

            return entry.GetValue();
        }

        public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? result)
        {
            Guard.IsNotNull(key, nameof(key));

            result = GetValue(key);

            return result.IsNotNull();
        }

        public TValue GetOrCreateValue(
            TKey key,
            Func<ICacheEntry<TValue>, TValue> factory)
        {
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(factory, nameof(factory));

            ICacheEntry<TValue> entry;

            if (entries.TryGetValue(key, out entry))
            {
                if (!entry.IsValid)
                    entry.SetValue(factory(entry));

                return entry.GetValue()!;
            }

            entry = CreateEntry(key);
            var value = factory(entry);
            entry.SetValue(value);

            return value;
        }

        public TValue GetOrCreateValue<TState>(
            TKey key,
            TState state,
            Func<ICacheEntry<TValue>, TState, TValue> factory
            )
        {
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(factory, nameof(factory));

            ICacheEntry<TValue> entry;

            if (entries.TryGetValue(key, out entry))
            {
                if (!entry.IsValid)
                    entry.SetValue(factory(entry, state));

                return entry.GetValue()!;
            }

            entry = CreateEntry(key);
            var value = factory(entry, state);
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

            if (!entries.TryGetValue(key, out entry)
                ||
                !entry.HasValue)
            {
                value = default;
                return false;
            }

            value = entry.GetValue()!;
            entry.SetValue(default);

            return entry.IsValid;
        }

        public bool TryAdd(
            TKey key,
            TValue value,
            [NotNullWhen(true)] out ICacheEntry<TValue>? entry)
        {
            Guard.IsNotNull(value, nameof(value));

            if (entries.TryGetValue(key, out entry))
            {
                entry.SetValue(value);
                return true;
            }

            entry = CreateEntry(key);
            entry.SetValue(value);

            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return entries.ContainsKey(key);
        }

        public void Clear()
        {
            lock (SyncRoot)
                entries.Clear();
        }

        public void Dispose() => timer?.Dispose();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var (key, entry) in entries)
            {
                if (entry.IsValid)
                    yield return KeyValuePair.Create(key, entry.GetValue()!);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void OnExpirationScaneFrequencyChanged(TimeSpan scanFreq)
        {
            if (scanFreq <= TimeSpan.Zero)
            {
                timer?.Dispose();
                timer = null;
                return;
            }

            timer = new Timer(ValidateEntries, null, TimeSpan.Zero, scanFreq);
        }

        private void ValidateEntries(object state)
        {
            if (SizeLimit is not null
                &&
                SizeLimit > 0
                &&
                entries.Count > SizeLimit.Value)
            {
                var toRemoveCount = entries.Count - SizeLimit.Value;

                var toRemove = entries.OrderByDescending(entry =>
                    {
                        return entry.Value.IdleTime.Ticks;
                    })
                    .ThenBy(entry =>
                    {
                        return !entry.Value.HasValue;
                    });

                int removedCount = 0;

                lock (SyncRoot)
                {
                    foreach (var (entryKey, _) in toRemove)
                    {
                        if (removedCount++ >= toRemoveCount)
                            break;

                        entries.TryRemove(entryKey, out _);
                    }
                }
            }

            lock (SyncRoot)
            {
                foreach (var (_, entry) in entries)
                {
                    if (entry.IsValid)
                        continue;

                    entry.SetValue(default);
                }
            }
        }
    }
}
