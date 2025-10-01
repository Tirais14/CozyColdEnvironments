using Cysharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable annotations
namespace CCEnvs.Language
{
    public readonly struct Seq<T> : IReadOnlyList<T>, IEquatable<Seq<T>>
    {
        public static Seq<T> Empty => new();

        public T Item1 { get; }
        public T Item2 { get; }
        public T Item3 { get; }
        public T Item4 { get; }
        public int Count { get; }
        public T this[int index] {
            get
            {
                if (index >= Count)
                    return CC.Throw.IndexOutOfRange(index).As<T>();

                return index switch
                {
                    0 => Item1,
                    1 => Item2,
                    2 => Item3,
                    3 => Item4,
                    _ => CC.Throw.IndexOutOfRange(index).As<T>(),
                };
            }
        }

        public Seq(T item1)
            :
            this()
        {
            Item1 = item1;
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

        public Seq<T> SetItem1(T item1) => new(item1, Item2, Item3, Item4);

        public Seq<T> SetItem2(T item2) => new(Item1, item2, Item3, Item4);

        public Seq<T> SetItem3(T item3) => new(Item1, Item2, item3, Item4);

        public Seq<T> SetItem4(T item4) => new(Item1, Item2, Item3, item4);

        public Seq<T> Reset() => Empty;

        public override string ToString()
        {
            var sb = ZString.CreateStringBuilder();

            sb.AppendJoin("; ", this);

            return sb.ToString();
        }

        public bool Equals(Seq<T> other)
        {
            var comparer = EqualityComparer<T>.Default;

            return comparer.Equals(Item1, other.Item1)
                   &&
                   comparer.Equals(Item2, other.Item2)
                   &&
                   comparer.Equals(Item3, other.Item3)
                   &&
                   comparer.Equals(Item4, other.Item4);
        }
        public override bool Equals(object obj)
        {
            return obj is Seq<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item1, Item2, Item3, Item4);
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
                if (pos >= seq.Count)
                {
                    Current = default!;
                    return false;
                }

                Current = seq[pos++];
                return true;
            }

            public void Reset()
            {
                pos = 0;
            }
        }
    }
}
