using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections.ObjectModel
{
    [Obsolete("Cause issues", true)]
    public class ListBase<T> : ReadOnlyListBase<T>, IList<T>
    {
        protected int pointer = -1;

        public override int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => pointer + 1;
        }

        new public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => base[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index >= Count || index >= inner.Length)
                    CC.Throw.IndexOutOfRange(index);

                inner[index] = value;
            }
        }

        protected int EmptySlotCount => Count - inner.Length;
        protected bool HasEmptySlot => EmptySlotCount > 0;

        bool ICollection<T>.IsReadOnly => false;

        public ListBase(int capacity) 
            :
            base(capacity)
        {
        }

        public ListBase()
        {
        }

        public ListBase(IEnumerable<T> collection)
            :
            base(collection)
        {
        }

        public ListBase(IEnumerable<T> collection, int capacity)
            :
            base(collection, capacity)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Add(T item)
        {
            if (!HasEmptySlot)
                IncreaseCapacity();

            inner[++pointer] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Clear()
        {
            pointer = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            return Array.IndexOf(inner, item) > -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            return Array.IndexOf(inner, item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Insert(int index, T item)
        {
            if (!HasEmptySlot)
                IncreaseCapacity();
            if (index >= Count || index >= inner.Length)
                CC.Throw.IndexOutOfRange(index);

            T replaced = inner[index];
            inner[index] = item;
            pointer++;

            int offset = index + 1;
            var segment = new Span<T>(inner, offset, Count - offset + 1);

            for (int i = 0; i < segment.Length; i++)
            {
                segment[i] = replaced;
                replaced = segment[i + 1];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Remove(T item)
        {
            int idx = IndexOf(item);
            if (idx < 0)
                return false;

            inner[idx] = default!;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void RemoveAt(int index)
        {
            if (index >= Count || index >= inner.Length)
                CC.Throw.IndexOutOfRange(index);
            if (index == pointer)
            {
                pointer--;
                return;
            }

            int offset = index - 1;
            var segment = new Span<T>(inner, offset, Count - offset);
            for (int i = 0; i < segment.Length; i++)
                segment[i] = segment[i + 1];

            pointer--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void TrimExcess()
        {
            var temp = new T[Count];

            inner.CopyTo(temp, 0);
            inner = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void IncreaseCapacity(int multiplier = 2)
        {
            var temp = new T[Capacity * multiplier];

            inner.CopyTo(temp, 0);
            inner = temp;
        }
    }
}
