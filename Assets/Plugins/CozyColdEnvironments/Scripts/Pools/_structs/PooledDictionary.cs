using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledDictionary<TDictionary, TKey, TValue, TState>
        :
        IDisposable,
        IDictionary<TKey, TValue>,
        IEquatable<PooledDictionary<TDictionary, TKey, TValue, TState>>

        where TDictionary : IDictionary<TKey, TValue>
    {
        private readonly TState state;
        private readonly Action<TDictionary, TState> disposeAction;

        public readonly TValue this[TKey key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Dictionary[key];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Dictionary[key] = value;
        }

        public readonly TDictionary Dictionary { get; }

        public readonly ICollection<TKey> Keys {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Dictionary.Keys;
        }

        public readonly ICollection<TValue> Values {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Dictionary.Values;
        }

        public readonly int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Dictionary.Count;
        }

        public bool IsInitialized { get; }

        readonly bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public PooledDictionary(
            TDictionary dictionary,
            TState state,
            Action<TDictionary, TState> disposeAction
            )
        {
            disposed = 0;

            Dictionary = dictionary;
            this.state = state;
            this.disposeAction = disposeAction;

            IsInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PooledDictionary<TDictionary, TKey, TValue, TState> left, PooledDictionary<TDictionary, TKey, TValue, TState> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PooledDictionary<TDictionary, TKey, TValue, TState> left, PooledDictionary<TDictionary, TKey, TValue, TState> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(KeyValuePair<TKey, TValue> item)
        {
            Dictionary.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Clear() => Dictionary.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(TKey key)
        {
            return Dictionary.Remove(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Remove(item.Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        private int disposed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposeAction is not null)
            {
                try
                {
                    disposeAction?.Invoke(Dictionary, state);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override bool Equals(object? obj)
        {
            return obj is PooledDictionary<TDictionary, TKey, TValue, TState> dictionary && Equals(dictionary);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(PooledDictionary<TDictionary, TKey, TValue, TState> other)
        {
            return EqualityComparer<TState?>.Default.Equals(state, other.state)
                   &&
                   disposeAction == other.disposeAction
                   &&
                   disposed == other.disposed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(state, disposeAction, disposed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        readonly void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
            KeyValuePair<TKey, TValue>[] array,
            int arrayIndex
            )
        {
            Dictionary.CopyTo(array, arrayIndex);
        }
    }

    public readonly struct PooledDictionary<TKey, TValue> 
        :
        IDictionary<TKey, TValue>,
        IDisposable,
        IEquatable<PooledDictionary<TKey, TValue>>
    {
        private readonly PooledDictionary<Dictionary<TKey, TValue>, TKey, TValue, PooledObject<Dictionary<TKey, TValue>>> core;

        public TValue this[TKey key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core[key];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => core[key] = value;
        }

        public Dictionary<TKey, TValue> Dictionary {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core.Dictionary;
        }

        public ICollection<TKey> Keys {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core.Keys;
        }

        public ICollection<TValue> Values {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core.Values;
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core.Count;
        }

        public bool IsInitialized {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core.IsInitialized;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public PooledDictionary(int? capacity)
        {
            if (capacity is null)
            {
                PooledObject<Dictionary<TKey, TValue>> handle = DictionaryPool<TKey, TValue>.Shared.Get();
                core = new PooledDictionary<Dictionary<TKey, TValue>, TKey, TValue, PooledObject<Dictionary<TKey, TValue>>>(
                    handle.Value,
                    handle,
                    static (_, handle) => handle.Dispose()
                    );
            }
            else
                core = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PooledDictionary<TKey, TValue> left, PooledDictionary<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PooledDictionary<TKey, TValue> left, PooledDictionary<TKey, TValue> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EnsureCapacity(int capacity)
        {
            return core.Dictionary.EnsureCapacity(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue value) => core.Add(key, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(KeyValuePair<TKey, TValue> item) => core.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => core.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(KeyValuePair<TKey, TValue> item) => core.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => core.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)core).CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key) => core.Remove(key);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(KeyValuePair<TKey, TValue> item) => core.Remove(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return core.TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => core.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is PooledDictionary<TKey, TValue> typed && Equals(typed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(PooledDictionary<TKey, TValue> other)
        {
            return core.Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return core.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return core.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => core.GetEnumerator();
    }

}
