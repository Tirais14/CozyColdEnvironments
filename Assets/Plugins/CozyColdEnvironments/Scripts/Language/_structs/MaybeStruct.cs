using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct MaybeStruct<T>
        : IEnumerable<T>,
        IEquatable<MaybeStruct<T>>,
        ITarget<MaybeStruct<T>, T>

        where T : struct
    {
        public static MaybeStruct<T> None => new();

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private bool m_hasValue;

        [UnityEngine.SerializeField]
        private T m_value;

        private readonly T? inner => m_hasValue ? m_value : null;
#else
        private readonly T? inner;
#endif

        public readonly bool IsSome => inner.HasValue;
        public readonly bool IsNone => !IsSome;

        public MaybeStruct(T value)
        {
#if UNITY_2017_1_OR_NEWER
            m_hasValue = true;
            m_value = value;
#else
            inner = value;
#endif
        }

        public static implicit operator MaybeStruct<T>(T source)
        {
            return new MaybeStruct<T>(source);
        }
        public static explicit operator T(MaybeStruct<T> source)
        {
            return source.inner.GetValueOrDefault();
        }

        public static bool operator ==(MaybeStruct<T> left, MaybeStruct<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaybeStruct<T> left, MaybeStruct<T> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> IfSome(Action<T> action)
        {
            return Lang.IfSome(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> IfNone<TOut>(Func<TOut> selector)
        {
            return Lang.IfNone<MaybeStruct<T>, T, TOut>(this, selector).Maybe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return Lang.Match(this, some, none).Maybe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut> selector)
        {
            return Lang.Map(this, selector).Maybe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Check(Predicate<T> predicate)
        {
            return Lang.Check(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CheckUnsafe(Predicate<T> predicate)
        {
            return Lang.CheckUnsafe(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access() => inner.GetValueOrDefault();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Access([NotNullWhen(true)] out T result)
        {
            result = m_value;

            return IsSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(T defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(Func<T> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<MaybeStruct<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Select<TOut>(Func<T, TOut> selector)
        {
            return Map(selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Where(Predicate<T> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsNone || !predicate(inner.GetValueOrDefault()))
                return None;

            return this;
        }

        public readonly bool Equals(MaybeStruct<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(inner, other.inner);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is MaybeStruct<T> typed && Equals(typed);
        }
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(inner);
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsNone)
                yield break;

            yield return inner!.Value;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
