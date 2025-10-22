using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct Ghost<T> : IEnumerable<T>, IEquatable<Ghost<T>>, IGhost<T>
    {
        public static Ghost<T> None => new();

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T value;
#else
        private readonly T value;
#endif

        public readonly bool IsNone => value.IsDefault();
        public readonly bool IsSome => !IsNone;

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

        public readonly Ghost<T> IfSome(Action<T> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsSome)
                action(value);

            return this;
        }
        public readonly Ghost<TOut> IfSome<TOut>(Func<T, TOut> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsSome)
                return action(value);

            return Ghost<TOut>.None;
        }

        public readonly Ghost<T> IfNone(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsNone)
                action();

            return this;
        }
        public readonly T IfNone(T defaultValue)
        {
            if (IsNone)
                return defaultValue;

            return value;
        }
        public readonly T IfNone(Func<T> defaultValueFactory)
        {
            Guard.IsNotNull(defaultValueFactory, nameof(defaultValueFactory));

            if (IsNone)
                return defaultValueFactory();

            return value;
        }

        public readonly Ghost<TOther> Map<TOther>(Func<T, TOther> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            return IsSome ? selector(value) : Ghost<TOther>.None;
        }

        public readonly Ghost<T> Match(Action<T> some, Action none)
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (IsSome)
                some(value);
            else
                none();

            return this;
        }
        public readonly Ghost<TOther> Match<TOther>(Func<T, TOther> some, Func<TOther> none)
        {
            Guard.IsNotNull(some, nameof(some));

            if (IsSome)
                return some(value);
            else
                return none is not null ? none() : default!;
        }

        public readonly T Value() => IfNone(default(T)!);

        /// <exception cref="CCException"></exception>
        public readonly T ValueUnsafe()
        {
            if (IsNone)
                throw new CCException($"{GetType().GetName()} is none.");

            return value;
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsNone)
                yield break;

            yield return value;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly bool Equals(Ghost<T> other)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Ghost<T> typed && Equals(typed);
        }
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}
