using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public static class Prioritized
    {
        public static Prioritized<TValue, TComparable> Create<TValue, TComparable>(
            TValue value,
            TComparable comparable,
            IEqualityComparer<TValue>? valueEqualityComparer = null,
            IEqualityComparer<TComparable>? comparableEqualityComparer = null)

            where TComparable : struct, IComparable<TComparable>
        {
            return new Prioritized<TValue, TComparable>(value, comparable, valueEqualityComparer, comparableEqualityComparer);
        }
    }

    public readonly struct Prioritized<TValue, TComparable>
        : IPrioritized,
        IComparable<Prioritized<TValue, TComparable>>,
        IEquatable<Prioritized<TValue, TComparable>>

        where TComparable : struct, IComparable<TComparable>
    {
        public TValue Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public TComparable Comparable { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public IEqualityComparer<TValue> ValueEqualityComparer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public IEqualityComparer<TComparable> ComparableEqualityComparer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        public Prioritized(
            TValue value,
            TComparable comparable,
            IEqualityComparer<TValue>? valueEqualityComparer = null,
            IEqualityComparer<TComparable>? comparableEqualityComparer = null)
        {
            Value = value;
            Comparable = comparable;
            ValueEqualityComparer = valueEqualityComparer ?? EqualityComparer<TValue>.Default;
            ComparableEqualityComparer = comparableEqualityComparer ?? EqualityComparer<TComparable>.Default;

        }

        public static explicit operator TValue(Prioritized<TValue, TComparable> source)
        {
            return source.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Prioritized<TValue, TComparable> left, Prioritized<TValue, TComparable> right)
        {
            return left.CompareTo(right) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Prioritized<TValue, TComparable> left, Prioritized<TValue, TComparable> right)
        {
            return left.CompareTo(right) < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Prioritized<TValue, TComparable> left, Prioritized<TValue, TComparable> right)
        {
            return left.CompareTo(right) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Prioritized<TValue, TComparable> left, Prioritized<TValue, TComparable> right)
        {
            return left.CompareTo(right) <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Prioritized<TValue, TComparable> left, Prioritized<TValue, TComparable> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Prioritized<TValue, TComparable> left, Prioritized<TValue, TComparable> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Prioritized<TValue, TComparable> other)
        {
            return ValueEqualityComparer.Equals(Value, other.Value)
                   &&
                   ComparableEqualityComparer.Equals(Comparable, other.Comparable)
                   &&
                   EqualityComparer<IEqualityComparer<TValue>>.Default.Equals(ValueEqualityComparer, other.ValueEqualityComparer);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Prioritized<TValue, TComparable> typed && Equals(typed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Comparable, ValueEqualityComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"({nameof(Value)}: {Value}; {nameof(Comparable)}: {Comparable})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Prioritized<TValue, TComparable> other)
        {
            return Comparable.CompareTo(other.Comparable);
        }
    }
}
