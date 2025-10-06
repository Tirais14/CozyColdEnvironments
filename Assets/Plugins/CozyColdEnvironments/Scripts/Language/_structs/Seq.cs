using Cysharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;

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
        public T Item5 { get; }
        public T Item6 { get; }
        public int Count { get; }
        public int Length => 6;
        public T this[int index] {
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

        public Seq(T item1, T item2, T item3, T item4, T item5)
            :
            this(item1, item2, item3, item4)
        {
            Item5 = item5;
        }

        public Seq(T item1, T item2, T item3, T item4, T item5, T item6)
            :
            this(item1, item2, item3, item4)
        {
            Item6 = item6;
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

        public Seq<T> SetItem1(T item1) => new(item1, Item2, Item3, Item4, Item5, Item6);

        public Seq<T> SetItem2(T item2) => new(Item1, item2, Item3, Item4, Item5, Item6);

        public Seq<T> SetItem3(T item3) => new(Item1, Item2, item3, Item4, Item5, Item6);

        public Seq<T> SetItem4(T item4) => new(Item1, Item2, Item3, item4, Item5, Item6);

        public Seq<T> SetItem5(T item5) => new(Item1, Item2, Item3, Item5, item5, Item6);

        public Seq<T> SetItem6(T item6) => new(Item1, Item2, Item3, Item4, Item5, item6);

        public Seq<T> Append(T item)
        {
            if (Count >= Length)
                throw new Diagnostics.LogicException("Sequence is fu");
        }

        public Seq<T> Reset() => Empty;

        public override string ToString()
        {
            var sb = ZString.CreateStringBuilder();

            sb.AppendJoin("; ", this.As<IEnumerable<T>>());

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
                   comparer.Equals(Item4, other.Item4)
                   &&
                   comparer.Equals(Item5, other.Item5)
                   &&
                   comparer.Equals(Item6, other.Item6);
        }
        public override bool Equals(object obj)
        {
            return obj is Seq<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item1, Item2, Item3, Item4, Item5, Item6);
        }

        public Seq<TOut> Cast<TOut>()
        {

        }

        public void For(Action<T, int> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            T item;
            for (int i = 0; i < Length; i++)
            {
                item = this[i];

                action(item, i);
            }
        }
        public void For<TOut>(Func<T, int, TOut> func, bool skipDefault)
        {
            CC.Guard.NullArgument(func, nameof(func));

            T item;
            for (int i = 0; i < Length; i++)
            {
                item = this[i];
                if (skipDefault && item.IsDefault())
                    continue;

                func(item, i);
            }
        }

        public void ForEach(Action<T> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            For((x, _) => action(x), skipDefault);
        }

        public T[] ToArray(bool skipDefault)
        {
            return ToList(skipDefault).ToArray();
        }
        public T[] ToArray() => ToArray(skipDefault: true);

        public List<T> ToList(bool skipDefault)
        {
            var list = new List<T>(Length);
            ForEach(x => list.Add(x), skipDefault);

            return list;
        }
        public List<T> ToList() => ToList(skipDefault: true);

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
