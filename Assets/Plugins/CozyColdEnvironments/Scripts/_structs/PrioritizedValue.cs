using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public readonly struct PrioritizedValue<T>
        : IPrioritized<int>,
        IComparable<PrioritizedValue<T>>,
        IEquatable<PrioritizedValue<T>>
    {
        public T Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public int Priority { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        public IEqualityComparer<T> ValueComparer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        public PrioritizedValue(T value, int priority, IEqualityComparer<T> valueComparer)
        {
            CC.Guard.NullArgument(valueComparer, nameof(valueComparer));

            Value = value;
            Priority = priority;
            ValueComparer = valueComparer;
        }

        public PrioritizedValue(T value, int priority)
            :
            this(value, priority, EqualityComparer<T>.Default)
        {
        }

        public PrioritizedValue(T value, IEqualityComparer<T> valueComparer)
            :
            this(value, priority: default, valueComparer)
        {
        }

        public PrioritizedValue(T value)
            :
            this(value, priority: default, EqualityComparer<T>.Default)
        {
        }

        public static explicit operator T(PrioritizedValue<T> source)
        {
            return source.Value;
        }

        public static bool operator >(PrioritizedValue<T> left, PrioritizedValue<T> right)
        {
            return left.Priority > right.Priority;
        }

        public static bool operator <(PrioritizedValue<T> left, PrioritizedValue<T> right)
        {
            return left.Priority < right.Priority;
        }

        public static bool operator >=(PrioritizedValue<T> left, PrioritizedValue<T> right)
        {
            return left.Priority >= right.Priority;
        }

        public static bool operator <=(PrioritizedValue<T> left, PrioritizedValue<T> right)
        {
            return left.Priority <= right.Priority;
        }

        public static bool operator ==(PrioritizedValue<T> left, PrioritizedValue<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PrioritizedValue<T> left, PrioritizedValue<T> right)
        {
            return !(left == right);
        }

        public bool Equals(PrioritizedValue<T> other)
        {
            return ValueComparer.Equals(Value, other.Value)
                   &&
                   Priority == other.Priority
                   &&
                   ValueComparer.Equals(other.ValueComparer);
        }
        public override bool Equals(object obj)
        {
            return obj is PrioritizedValue<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Priority, ValueComparer);
        }

        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}; {nameof(Priority)}: {Priority}";
        }

        public int CompareTo(PrioritizedValue<T> other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}
