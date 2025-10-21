using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Language
{
    public readonly struct Ghost<T> : IEnumerable<T>, IEquatable<Ghost<T>>
    {
        public static Ghost<T> None => new();

        private readonly T value;

        public bool IsNone => value.IsDefault();
        public bool IsSome => !IsNone;

        public Ghost(T value)
        {
            this.value = value;
        }

        public static implicit operator Ghost<T>(T source)
        {
            return new Ghost<T>(source);
        }
        public static explicit operator T(Ghost<T> source)
        {
            return source.value;
        }

        public static bool operator ==(Ghost<T> left, Ghost<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ghost<T> left, Ghost<T> right)
        {
            return !(left == right);
        }

        public Ghost<T> IfSome(Action<T> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsSome)
                action(value);

            return this;
        }

        public Ghost<T> IfNone(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsNone)
                action();

            return this;
        }

        public Ghost<TOther> Map<TOther>(Func<T, TOther> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            return IsSome ? selector(value) : Ghost<TOther>.None;
        }

        public Ghost<T> Match(Action<T> some, Action none)
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (IsSome)
                some(value);
            else
                none();

            return this;
        }
        public Ghost<TOther> Match<TOther>(Func<T, TOther> some, Func<TOther> none)
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
            if (IsNone)
                yield break;

            yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Ghost<T> other)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }
        public override bool Equals(object obj)
        {
            return obj is Ghost<T> typed && Equals(typed);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}
