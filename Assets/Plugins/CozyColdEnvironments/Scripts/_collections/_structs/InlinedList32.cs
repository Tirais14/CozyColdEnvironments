using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Collections
{
    public struct InlinedList32<T> : IList<T>, IEquatable<InlinedList32<T>>
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly InlinedList32<T> list;

            private int index;

            public T Current { get; private set; }

            readonly object IEnumerator.Current => Current!;

            public Enumerator(in InlinedList32<T> list)
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
        private T? item16;
        private T? item17;
        private T? item18;
        private T? item19;
        private T? item20;
        private T? item21;
        private T? item22;
        private T? item23;
        private T? item24;
        private T? item25;
        private T? item26;
        private T? item27;
        private T? item28;
        private T? item29;
        private T? item30;
        private T? item31;

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
                    16 => item16!,
                    17 => item17!,
                    18 => item18!,
                    19 => item19!,
                    20 => item20!,
                    21 => item21!,
                    22 => item22!,
                    23 => item23!,
                    24 => item24!,
                    25 => item25!,
                    26 => item26!,
                    27 => item27!,
                    28 => item28!,
                    29 => item29!,
                    30 => item30!,
                    31 => item31!,
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
                    case 16:
                        item16 = value;
                        break;
                    case 17:
                        item17 = value;
                        break;
                    case 18:
                        item18 = value;
                        break;
                    case 19:
                        item19 = value;
                        break;
                    case 20:
                        item20 = value;
                        break;
                    case 21:
                        item21 = value;
                        break;
                    case 22:
                        item22 = value;
                        break;
                    case 23:
                        item23 = value;
                        break;
                    case 24:
                        item24 = value;
                        break;
                    case 25:
                        item25 = value;
                        break;
                    case 26:
                        item26 = value;
                        break;
                    case 27:
                        item27 = value;
                        break;
                    case 28:
                        item28 = value;
                        break;
                    case 29:
                        item29 = value;
                        break;
                    case 30:
                        item30 = value;
                        break;
                    case 31:
                        item31 = value;
                        break;
                    default:
                        throw CC.ThrowHelper.IndexOutOfRangeException(index);
                }
            }
        }

        public readonly int Length => length;
        public readonly int Capacity => 32;
        public readonly int FreeSpace => Capacity - length;

        readonly bool ICollection<T>.IsReadOnly => false;

        readonly int ICollection<T>.Count => length;

        public static bool operator ==(InlinedList32<T> left, InlinedList32<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InlinedList32<T> left, InlinedList32<T> right)
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
            item16 = default;
            item17 = default;
            item18 = default;
            item19 = default;
            item20 = default;
            item21 = default;
            item22 = default;
            item23 = default;
            item24 = default;
            item25 = default;
            item26 = default;
            item27 = default;
            item28 = default;
            item29 = default;
            item30 = default;
            item31 = default;

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
            return obj is InlinedList32<T> list && Equals(list);
        }

        public readonly bool Equals(InlinedList32<T> other)
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
            hash.Add(item16);
            hash.Add(item17);
            hash.Add(item18);
            hash.Add(item19);
            hash.Add(item20);
            hash.Add(item21);
            hash.Add(item22);
            hash.Add(item23);
            hash.Add(item24);
            hash.Add(item25);
            hash.Add(item26);
            hash.Add(item27);
            hash.Add(item28);
            hash.Add(item29);
            hash.Add(item30);
            hash.Add(item31);
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
                throw new InvalidOperationException("Collection is full");
        }
    }
}
