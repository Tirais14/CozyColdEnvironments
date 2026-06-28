#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CCEnvs.Collections
{
    public struct InlinedList16<T> : IList<T>, IEquatable<InlinedList16<T>>
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly InlinedList16<T> list;

            private int index;

            public T Current { get; private set; }

            readonly object IEnumerator.Current => Current!;

            public Enumerator(in InlinedList16<T> list)
            {
                index = -1;
                Current = default!;

                this.list = list;
            }

            public bool MoveNext()
            {
                if (++index >= list.length)
                    return false;

                Current = list[index];
                return true;
            }

            public readonly void Dispose() { }

            public void Reset() => index = -1;

            public readonly Enumerator GetEnumerator() => this;
        }

        private T? item0;
        private T? item1;
        private T? item2;
        private T? item3;
        private T? item4;
        private T? item5;
        private T? item6;
        private T? item7;
        private T? item8;
        private T? item9;
        private T? item10;
        private T? item11;
        private T? item12;
        private T? item13;
        private T? item14;
        private T? item15;

        private int length;

        public T this[int index] {
            readonly get
            {
                if (index >= Length)
                    throw CC.ThrowHelper.IndexOutOfRangeException(index);

                return index switch
                {
                    0 => item0!,
                    1 => item1!,
                    2 => item2!,
                    3 => item3!,
                    4 => item4!,
                    5 => item5!,
                    6 => item6!,
                    7 => item7!,
                    8 => item8!,
                    9 => item9!,
                    10 => item10!,
                    11 => item11!,
                    12 => item12!,
                    13 => item13!,
                    14 => item14!,
                    15 => item15!,
                    _ => throw CC.ThrowHelper.IndexOutOfRangeException(index)
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        item0 = value;
                        break;
                    case 1:
                        item1 = value;
                        break;
                    case 2:
                        item2 = value;
                        break;
                    case 3:
                        item3 = value;
                        break;
                    case 4:
                        item4 = value;
                        break;
                    case 5:
                        item5 = value;
                        break;
                    case 6:
                        item6 = value;
                        break;
                    case 7:
                        item7 = value;
                        break;
                    case 8:
                        item8 = value;
                        break;
                    case 9:
                        item9 = value;
                        break;
                    case 10:
                        item10 = value;
                        break;
                    case 11:
                        item11 = value;
                        break;
                    case 12:
                        item12 = value;
                        break;
                    case 13:
                        item13 = value;
                        break;
                    case 14:
                        item14 = value;
                        break;
                    case 15:
                        item15 = value;
                        break;
                    default:
                        throw CC.ThrowHelper.IndexOutOfRangeException(index);
                }
            }
        }

        public readonly int Length => length;
        public readonly int Capacity => 16;
        public readonly int FreeSpace => Capacity - length;

        readonly bool ICollection<T>.IsReadOnly => false;

        readonly int ICollection<T>.Count => length;

        public static bool operator ==(InlinedList16<T> left, InlinedList16<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InlinedList16<T> left, InlinedList16<T> right)
        {
            return !(left == right);
        }

        public void Add(T item)
        {
            ThrowOnCapacityZero();
            this[length++] = item;
        }

        public bool AddIfNotContainer(T item)
        {
            if (Contains(item))
                return false;

            Add(item);
            return true;
        }

        public void Clear()
        {
            item0 = default;
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
            item5 = default;
            item6 = default;
            item7 = default;
            item8 = default;
            item9 = default;
            item10 = default;
            item11 = default;
            item12 = default;
            item13 = default;
            item14 = default;
            item15 = default;

            length = 0;
        }

        public readonly bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            Guard.IsNotNull(array);

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < length)
                throw new ArgumentException("Not enough space in array");

            for (int i = 0; i < length; i++)
                array[arrayIndex + i] = this[i];
        }

        public readonly int IndexOf(T item)
        {
            for (int i = 0; i < length; i++)
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                    return i;

            return -1;
        }

        public void Insert(int index, T item)
        {
            ThrowOnCapacityZero();

            if (index < 0 || index > length)
                throw CC.ThrowHelper.IndexOutOfRangeException(index);

            for (int i = length - 1; i >= index; i--)
                this[i + 1] = this[i];

            length++;
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (index >= length || index <= -1)
                throw CC.ThrowHelper.IndexOutOfRangeException(index);

            if (index != length - 1)
            {
                for (int i = index + 1; i < length; i++)
                    this[i - 1] = this[i];

                this[--length] = default!;
            }
            else
            {
                this[index] = default!;
                length--;
            }
        }

        public bool Remove(T item)
        {
            int itemIdx = IndexOf(item);

            if (itemIdx == -1)
                return false;

            RemoveAt(itemIdx);
            return true;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is InlinedList16<T> list && Equals(list);
        }

        public readonly bool Equals(InlinedList16<T> other)
        {
            if (length != other.length)
                return false;

            for (int i = 0; i < length; i++)
                if (!EqualityComparer<T?>.Default.Equals(this[i], other[i]))
                    return false;

            return true;
        }

        public readonly override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(item0);
            hash.Add(item1);
            hash.Add(item2);
            hash.Add(item3);
            hash.Add(item4);
            hash.Add(item5);
            hash.Add(item6);
            hash.Add(item7);
            hash.Add(item8);
            hash.Add(item9);
            hash.Add(item10);
            hash.Add(item11);
            hash.Add(item12);
            hash.Add(item13);
            hash.Add(item14);
            hash.Add(item15);
            hash.Add(length);
            return hash.ToHashCode();
        }

        public readonly override string ToString()
        {
            return this.SequenceToString();
        }

        public readonly IEnumerator<T> GetEnumerator() => new Enumerator(this);
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly void ThrowOnCapacityZero()
        {
            if (FreeSpace == 0)
                throw new InvalidOperationException("Space is out");
        }
    }
}
