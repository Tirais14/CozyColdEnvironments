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
        struct Ghost<T>
        : IEnumerable<T>,
        IEquatable<Ghost<T>>,
        IConditional<T>
    {
        public static Ghost<T> None => new();

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? inner;
#else
        private readonly T? inner;
#endif

        public readonly bool IsSome => inner.IsNotDefault();
        public readonly bool IsNone => !IsSome;

        public Ghost(T value)
        {
            this.inner = value;
        }

        public static implicit operator Ghost<T>(T source)
        {
            return new Ghost<T>(source);
        }
        public static explicit operator T?(Ghost<T> source)
        {
            return source.inner;
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
            return Lang.IfSome(this, action);
        }

        //public readonly Ghost<T> IfNone(Action action)
        //{
        //    return Lang.IfNone(this, action);
        //}

        public readonly Ghost<TOut> IfNone<TOut>(Func<TOut> selector)
        {
            return Lang.IfNone<Ghost<T>, T, TOut>(this, selector).AsGhost();
        }

        public readonly Ghost<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        public readonly Ghost<TOut> Match<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return Lang.Match(this, some, none).AsGhost();
        }

        public readonly Ghost<TOut> Map<TOut>(Func<T, TOut> selector)
        {
            return Lang.Map(this, selector).AsGhost();
        }

        public readonly T? Value() => inner;

        public readonly T? Value(T? defaultValue)
        {
            return Lang.Value(this, defaultValue);
        }

        public readonly T ValueUnsafe()
        {
            return Lang.ValueUnsafe<Ghost<T>, T>(this);
        }

        //public readonly Ghost<T> Map<TOut>(Func<T, TOut> selector)

        public readonly bool Equals(Ghost<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(inner, other.inner);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Ghost<T> typed && Equals(typed);
        }
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(inner);
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsNone)
                yield break;

            yield return inner!;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
