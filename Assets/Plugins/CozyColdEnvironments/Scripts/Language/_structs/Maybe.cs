using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Language
{
    [Serializable]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct Maybe<T>
        : IEquatable<Maybe<T>>,
        IConditional<Maybe<T>, T>
    {
        public static Maybe<T> None => new();

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? inner;
#else
        private readonly T? inner;
#endif

        public readonly bool IsSome => inner.IsNotDefault();
        public readonly bool IsNone => !IsSome;

        public Maybe(T value)
        {
            inner = value;
        }

        public static implicit operator Maybe<T>(T? source)
        {
            return new Maybe<T>(source!);
        }

        public static explicit operator T?(Maybe<T> source)
        {
            return source.inner;
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Catch() => inner!;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> IfSome(Action<T> action)
        {
            return Lang.IfSome(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none)
        {
            return Lang.Match(this, some, none);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut?> selector)
        {
            return Lang.Map(this, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> MapUnsafe<TOut>(Func<T?, TOut?> selector)
        {
            return Lang.MapUnsafe(this, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Check(T? value)
        {
            return Lang.Check(this, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Check(Predicate<T> predicate)
        {
            return Lang.Check(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CheckUnsafe(Predicate<T?> predicate)
        {
            return Lang.CheckUnsafe(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access() => inner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access(T? defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Access([NotNullWhen(true)] out T? result)
        {
            result = inner;

            return IsSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access(Func<T?> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<Maybe<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Apply(T? value)
        {
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Select<TOut>(Func<T, TOut?> selector)
        {
            return Map(selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Where(Predicate<T> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsNone || !predicate(inner!))
                return None;

            return this;
        }

        public readonly bool Equals(Maybe<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(inner, other.inner);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Maybe<T> typed && Equals(typed);
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

        IConditional IConditional.IfNone(Func<object> selector)
        {
            if (IsSome)
                return this;

            return selector().Maybe();
        }
    }
}
