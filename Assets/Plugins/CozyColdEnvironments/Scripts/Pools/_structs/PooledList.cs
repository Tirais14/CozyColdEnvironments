using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledList<TList, TValue, TState>
        :
        IDisposable,
        IList<TValue>, 
        IReadOnlyList<TValue>,
        IEquatable<PooledList<TList, TValue, TState>>
        
        where TList : IList<TValue>
    {
        private readonly TState? state;
        private readonly Action<TList, TState?> returnAction;

        public readonly TValue this[int index] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => guardedSelf.Value[index];
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => guardedSelf.Value[index] = value;
        }

        public TList Value { get; }

        public readonly int Count {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => guardedSelf.Value.Count;
        }

        public readonly bool IsInitialized { get; }
        public readonly bool IsReadOnly {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => guardedSelf.Value.IsReadOnly;
        }

        private readonly PooledList<TList, TValue, TState> guardedSelf {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!IsInitialized)
                    throw new InvalidOperationException($"Is not initialized");

                return this;
            }
        }

        public PooledList(
            TList list,
            TState? state,
            Action<TList, TState?> returnAction
            )
            :
            this()
        {
            Value = list;
            this.state = state;
            this.returnAction = returnAction;

            IsInitialized = true;
        }

        public static bool operator ==(PooledList<TList, TValue, TState> left, PooledList<TList, TValue, TState> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PooledList<TList, TValue, TState> left, PooledList<TList, TValue, TState> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(TValue item)
        {
            if (!IsInitialized)
                return -1;

            return Value.IndexOf(item);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Insert(int index, TValue item)
        {
            guardedSelf.Value.Insert(index, item);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void RemoveAt(int index) => guardedSelf.Value.RemoveAt(index);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(TValue item) => guardedSelf.Value.Add(item);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Clear()
        {
            if (!IsInitialized)
                return;

            Value.Clear();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(TValue item)
        {
            if (!IsInitialized)
                return false;

            return Value.Contains(item);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(TValue[] array, int arrayIndex)
        {
            guardedSelf.Value.CopyTo(array, arrayIndex);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(TValue item)
        {
            if (!IsInitialized)
                return false;

            return Value.Remove(item);
        }

        public readonly IEnumerator<TValue> GetEnumerator()
        {
            if (!IsInitialized)
                return Array.Empty<TValue>().GetEnumeratorT();

            return Value.GetEnumerator();
        }
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            try
            {
                returnAction?.Invoke(Value, state);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is PooledList<TList, TValue, TState> list && Equals(list);
        }

        public readonly bool Equals(PooledList<TList, TValue, TState> other)
        {
            return IsInitialized == other.IsInitialized
                   &&
                   disposed == other.disposed
                   &&
                   EqualityComparer<TState?>.Default.Equals(state, other.state) 
                   &&
                   EqualityComparer<Action<TList, TState?>>.Default.Equals(returnAction, other.returnAction) 
                   &&
                   EqualityComparer<TList>.Default.Equals(Value, other.Value)
                   &&
                   Count == other.Count
                   &&
                   IsReadOnly == other.IsReadOnly;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(
                IsInitialized,
                disposed,
                state,
                returnAction,
                Value,
                Count,
                IsReadOnly
                );
        }
    }

    public struct PooledList<TValue>
        :
        IDisposable,
        IList<TValue>,
        IReadOnlyList<TValue>
    {
        private readonly PooledList<List<TValue>, TValue, PooledObject<List<TValue>>> core;

        public readonly TValue this[int index] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => guardedSelf.Value[index];
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => guardedSelf.Value[index] = value;
        }

        public readonly int Count {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => guardedSelf.Value.Count;
        }

        public readonly bool IsInitialized { get; }

        public readonly List<TValue> Value {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => core.Value;
        }

        private readonly PooledList<TValue> guardedSelf {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!IsInitialized)
                    throw new InvalidOperationException($"Is not initialized");

                return this;
            }
        }

        readonly bool ICollection<TValue>.IsReadOnly {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => guardedSelf.Value.CastTo<ICollection<TValue>>().IsReadOnly;
        }

        public PooledList(int? capacity)
        {
            var listHandle = ListPool<TValue>.Shared.Get();

            if (capacity .HasValue)
                listHandle.Value.TryIncreaseCapacity(capacity.Value);

            core = new PooledList<List<TValue>, TValue, PooledObject<List<TValue>>>(
                listHandle.Value,
                listHandle,
                static (_, handle) => handle.Dispose()
                );

            IsInitialized = true;
        }

        public static PooledList<TValue> Create()
        {
            return new PooledList<TValue>(null);
        }

        public static implicit operator List<TValue>(PooledList<TValue> instance)
        {
            return instance.Value;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(TValue item)
        {
            if (!IsInitialized)
                return -1;

            return Value.IndexOf(item);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Insert(int index, TValue item)
        {
            guardedSelf.Value.Insert(index, item);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void RemoveAt(int index) => guardedSelf.Value.RemoveAt(index);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(TValue item) => guardedSelf.Value.Add(item);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Clear()
        {
            if (!IsInitialized)
                return;

            Value.Clear();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(TValue item)
        {
            if (!IsInitialized)
                return false;

            return Value.Contains(item);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(TValue[] array, int arrayIndex)
        {
            guardedSelf.Value.CopyTo(array, arrayIndex);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(TValue item)
        {
            if (!IsInitialized)
                return false;

            return Value.Remove(item);
        }

        public readonly IEnumerator<TValue> GetEnumerator()
        {
            if (!IsInitialized)
                return Array.Empty<TValue>().GetEnumeratorT();

            return Value.GetEnumerator();
        }
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly void Dispose() => core.Dispose();
    }
}
