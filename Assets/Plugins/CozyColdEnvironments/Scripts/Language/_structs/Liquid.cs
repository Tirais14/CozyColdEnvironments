#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable S3236
namespace CCEnvs.Language
{
    public readonly struct Liquid<T> : IEnumerable<T>, IEquatable<Liquid<T>>
    {
        public static Liquid<T> None => new();

        private readonly T value;

        public bool IsNone => value.IsDefault();
        public bool IsSome => !IsNone;

        public Liquid(T value)
        {
            this.value = value;
        }

        public static implicit operator Liquid<T>(T source)
        {
            return new Liquid<T>(source);
        }
        public static explicit operator T(Liquid<T> source)
        {
            return source.value;
        }

        public static bool operator ==(Liquid<T> left, Liquid<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Liquid<T> left, Liquid<T> right)
        {
            return !(left == right);
        }

        public Liquid<T> IfSome(Action<T> action)
        {
            Guard.IsNotNull(action, nameof(action));

            action(value);

            return this;
        }

        public Liquid<T> IfNone(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            action();

            return this;
        }

        public Liquid<TOther> Map<TOther>(Func<T, TOther> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            return IsSome ? selector(value) : Liquid<TOther>.None;
        }

        public Liquid<T> Match(Action<T> some, Action? none = null)
        {
            Guard.IsNotNull(some, nameof(some));

            if (IsSome)
                some(value);
            else
                none?.Invoke();

            return this;
        }
        public Liquid<TOther> Match<TOther>(Func<T, TOther> some, Func<TOther>? none)
        {
            Guard.IsNotNull(some, nameof(some));

            if (IsSome)
                return some(value);
            else
                return none is not null ? none() : default!;
        }

        public T ValueUnsafe() => value;

        public IEnumerator<T> GetEnumerator()
        {
            yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Liquid<T> other)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }
        public override bool Equals(object obj)
        {
            return obj is Liquid<T> typed && Equals(typed);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}
