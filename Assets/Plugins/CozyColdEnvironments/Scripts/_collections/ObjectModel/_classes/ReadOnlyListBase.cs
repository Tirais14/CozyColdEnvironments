#nullable enable
using SuperLinq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CCEnvs.Collections.ObjectModel
{
    public abstract class ReadOnlyListBase<T> : IReadOnlyList<T>
    {
        protected T[] inner;

        internal T[] ArrayInternal => inner;

        public int Capacity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Length;
        }
        public abstract int Count { get; }

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= Count || index >= inner.Length)
                    CC.Throw.IndexOutOfRange(index);

                return inner[index];
            }
        }

        protected ReadOnlyListBase(int capacity)
        {
            inner = new T[capacity];
        }

        protected ReadOnlyListBase()
            :
            this(4)
        {
        }

        protected ReadOnlyListBase(IEnumerable<T> collection)
        {
            inner = collection.ToArray();
        }

        protected ReadOnlyListBase(IEnumerable<T> collection, int capacity)
            :
            this(capacity)
        {
            collection.CopyTo(inner, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T[] ToArray()
        {
            var array = new T[Count];

            inner.CopyTo(array, 0);

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual List<T> ToList()
        {
            var list = new List<T>(Count);

            foreach (var item in inner)
                list.Add(item);

            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => inner.GetEnumeratorT();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
    }
}
