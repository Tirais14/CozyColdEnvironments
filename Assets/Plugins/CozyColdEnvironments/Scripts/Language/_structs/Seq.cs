using Cysharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable annotations
#pragma warning disable S3267
namespace CCEnvs.Language
{
    /// <summary>
    /// Has only 6 slots.
    /// Functional analog of array, to minimize memory allocations.
    /// Equals by items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Seq<T> : IReadOnlyList<T>, IEquatable<Seq<T>>
    {
        public static Seq<T> Empty => new();

        public readonly T Item1;
        public readonly T Item2;
        public readonly T Item3;
        public readonly T Item4;
        public readonly T Item5;
        public readonly T Item6;
        public readonly int Count;
        public readonly int Length => 6;
        public readonly bool IsEmpty => Count == 0;
        public IEqualityComparer<T> EqualityComparer { get; set; }
        public readonly T this[int index] {
            get
            {
                if (index >= Count)
                    CC.Throw.IndexOutOfRange(index);

                return index switch
                {
                    0 => Item1,
                    1 => Item2,
                    2 => Item3,
                    3 => Item4,
                    4 => Item5,
                    5 => Item6,
                    _ => CC.Throw.IndexOutOfRange(index).As<T>(),
                };
            }
        }

        readonly int IReadOnlyCollection<T>.Count => Count;

        public Seq(T item1)
            :
            this()
        {
            Item1 = item1;
            EqualityComparer = EqualityComparer<T>.Default;
        }

        public Seq(T item1, T item2)
            :
            this(item1)
        {
            Item2 = item2;
        }

        public Seq(T item1, T item2, T item3)
            :
            this(item1, item2)
        {
            Item3 = item3;
        }

        public Seq(T item1, T item2, T item3, T item4)
            :
            this(item1, item2, item3)
        {
            Item4 = item4;
        }

        public Seq(T item1, T item2, T item3, T item4, T item5)
            :
            this(item1, item2, item3, item4)
        {
            Item5 = item5;
        }

        public Seq(T item1, T item2, T item3, T item4, T item5, T item6)
            :
            this(item1, item2, item3, item4, item5)
        {
            Item6 = item6;
        }

        public static explicit operator (T, T)(Seq<T> source)
        {
            return (source.Item1, source.Item2);
        }

        public static explicit operator (T, T, T)(Seq<T> source)
        {
            return (source.Item1, source.Item2, source.Item3);
        }

        public static explicit operator (T, T, T, T)(Seq<T> source)
        {
            return (source.Item1, source.Item2, source.Item3, source.Item4);
        }

        public static explicit operator (T, T, T, T, T)(Seq<T> source)
        {
            return (source.Item1, source.Item2, source.Item3, source.Item4, source.Item5);
        }

        public static explicit operator (T, T, T, T, T, T)(Seq<T> source)
        {
            return (source.Item1, source.Item2, source.Item3, source.Item4, source.Item5, source.Item6);
        }

        public static explicit operator T[](Seq<T> source)
        {
            return source.ToArray();
        }

        public static bool operator ==(Seq<T> left, Seq<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Seq<T> left, Seq<T> right)
        {
            return !(left == right);
        }

        public readonly Seq<T> SetItem1(T item1)
        {
            return Count switch
            {
                0 => new Seq<T>(item1),
                1 => new Seq<T>(item1, Item2),
                2 => new Seq<T>(item1, Item2, Item3),
                3 => new Seq<T>(item1, Item2, Item3, Item4),
                4 => new Seq<T>(item1, Item2, Item3, Item4, Item5),
                5 => new Seq<T>(item1, Item2, Item3, Item4, Item5, Item6),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> SetItem2(T item2)
        {
            return Count switch
            {
                1 => new Seq<T>(Item1, item2),
                2 => new Seq<T>(Item1, item2, Item3),
                3 => new Seq<T>(Item1, item2, Item3, Item4),
                4 => new Seq<T>(Item1, item2, Item3, Item4, Item5),
                5 => new Seq<T>(Item1, item2, Item3, Item4, Item5, Item6),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> SetItem3(T item3)
        {
            return Count switch
            {
                2 => new Seq<T>(Item1, Item2, item3),
                3 => new Seq<T>(Item1, Item2, item3, Item4),
                4 => new Seq<T>(Item1, Item2, item3, Item4, Item5),
                5 => new Seq<T>(Item1, Item2, item3, Item4, Item5, Item6),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> SetItem4(T item4)
        {
            return Count switch
            {
                3 => new Seq<T>(Item1, Item2, Item3, item4),
                4 => new Seq<T>(Item1, Item2, Item3, item4, Item5),
                5 => new Seq<T>(Item1, Item2, Item3, item4, Item5, Item6),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> SetItem5(T item5)
        {
            return Count switch
            {
                4 => new Seq<T>(Item1, Item2, Item3, Item4, item5),
                5 => new Seq<T>(Item1, Item2, Item3, Item4, item5, Item6),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> SetItem6(T item6)
        {
            return Count switch
            {
                5 => new Seq<T>(Item1, Item2, Item3, Item4, Item5, item6),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> SetValue(int index, T item)
        {
            return index switch
            {
                0 => SetItem1(item),
                1 => SetItem2(item),
                2 => SetItem3(item),
                3 => SetItem4(item),
                4 => SetItem5(item),
                5 => SetItem6(item),
                _ => CC.Throw.IndexOutOfRange(index).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> Append(T item)
        {
            if (Count >= Length)
                throw new Diagnostics.LogicException("Cannot add item. Sequence is full.");

            return Count switch
            {
                0 => SetItem1(item),
                1 => SetItem2(item),
                2 => SetItem3(item),
                3 => SetItem4(item),
                4 => SetItem5(item),
                5 => SetItem6(item),
                _ => CC.Throw.IndexOutOfRange(Count).As<Seq<T>>(),
            };
        }

        public readonly Seq<T> Remove(T toRemove)
        {
            if (IsEmpty)
                return Empty;

            Seq<T> result = Empty;
            foreach (var item in this)
            {
                if (EqualityComparer.Equals(item, toRemove))
                    continue;

                result.Append(item);
            }

            return result;
        }
        public readonly Seq<T> RemoveAt(int index)
        {
            if (IsEmpty)
                return Empty;

            Seq<T> result = Empty;
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;

                result.Append(this[i]);
            }

            return result;
        }

        public readonly int IndexOf(T value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (EqualityComparer.Equals(value, this[i]))
                    return i;
            }

            return -1;
        }

        public readonly Seq<T> Clear() => Empty;

        public readonly override string ToString()
        {
            var sb = ZString.CreateStringBuilder();

            sb.AppendJoin("; ", this.As<IEnumerable<T>>());

            return sb.ToString();
        }

        public readonly bool Equals(Seq<T> other)
        {
            return EqualityComparer.Equals(Item1, other.Item1)
                   &&
                   EqualityComparer.Equals(Item2, other.Item2)
                   &&
                   EqualityComparer.Equals(Item3, other.Item3)
                   &&
                   EqualityComparer.Equals(Item4, other.Item4)
                   &&
                   EqualityComparer.Equals(Item5, other.Item5)
                   &&
                   EqualityComparer.Equals(Item6, other.Item6);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Seq<T> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Item1, Item2, Item3, Item4, Item5, Item6);
        }

        public readonly void For(Action<T, int> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            for (int i = 0; i < Count; i++)
                action(this[i], i);
        }
        public readonly Seq<TOut> For<TOut>(Func<T, int, TOut> func)
        {
            CC.Guard.NullArgument(func, nameof(func));

            var result = Seq<TOut>.Empty;
            for (int i = 0; i < Count; i++)
                result = result.Append(func(this[i], i));

            return result;
        }

        public readonly void ForEach(Action<T> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            foreach (var item in this)
                action(item);
        }
        public readonly Seq<TOut> ForEach<TOut>(Func<T, TOut> func)
        {
            CC.Guard.NullArgument(func, nameof(func));

            var result = Seq<TOut>.Empty;
            foreach (var item in this)
                result.Append(func(item));

            return result;
        }

        public readonly bool Contains(T value)
        {
            if (IsEmpty)
                return false;

            foreach (var item in this)
            {
                if (EqualityComparer.Equals(item, value))
                    return true;
            }

            return false;
        }

        public readonly bool All(Predicate<T> predicate)
        {
            CC.Guard.NullArgument(predicate, nameof(predicate));
            if (IsEmpty)
                return false;

            foreach (var item in this)
            {
                if (!predicate(item))
                    return false;
            }

            return true;
        }

        public readonly bool Any(Predicate<T> predicate)
        {
            CC.Guard.NullArgument(predicate, nameof(predicate));
            if (IsEmpty)
                return false;

            foreach (var item in this)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        public readonly Seq<TOut> Cast<TOut>()
        {
            return ForEach((x) => x.As<TOut>());
        }

        public readonly T[] ToArray()
        {
            var arr = new T[Count];
            for (int i = 0; i < Count; i++)
                arr[i] = this[i];

            return arr;
        }

        public readonly List<T> ToList()
        {
            var list = new List<T>(Count);
            for (int i = 0; i < Count; i++)
                list[i] = this[i];

            return list;
        }

        public readonly IEnumerator<T> GetEnumerator() => new Enumerator(this);

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly Seq<T> seq;
            private int pos;

            public T Current { readonly get; private set; }

            readonly object IEnumerator.Current => Current!;

            public Enumerator(Seq<T> seq)
            {
                this.seq = seq;

                pos = default;
                Current = default!;
            }

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (pos < seq.Count)
                {
                    Current = seq[pos++];
                    if (Current.IsNotDefault())
                        break;
                }

                if (pos >= seq.Count && Current.IsDefault())
                    return false;

                return true;
            }

            public void Reset()
            {
                pos = 0;
            }
        }
    }
}
