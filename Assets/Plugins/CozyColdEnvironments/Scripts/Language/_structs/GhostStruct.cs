using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct GhostStruct<T>
        : IEnumerable<T>,
        IEquatable<GhostStruct<T>>,
        ITarget<GhostStruct<T>, T>

        where T : struct
    {
        public static GhostStruct<T> None => new();

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

        public GhostStruct(T value)
        {
#if UNITY_2017_1_OR_NEWER
            m_hasValue = true;
            m_value = value;
#else
            inner = value;
#endif
        }

        public static implicit operator GhostStruct<T>(T source)
        {
            return new GhostStruct<T>(source);
        }
        public static explicit operator T(GhostStruct<T> source)
        {
            return source.inner.GetValueOrDefault();
        }

        public static bool operator ==(GhostStruct<T> left, GhostStruct<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GhostStruct<T> left, GhostStruct<T> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GhostStruct<T> IfSome(Action<T> action)
        {
            return Lang.IfSome(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GhostStruct<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Ghost<TOut> IfNone<TOut>(Func<TOut> selector)
        {
            return Lang.IfNone<GhostStruct<T>, T, TOut>(this, selector).AsGhost();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GhostStruct<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Ghost<TOut> Match<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return Lang.Match(this, some, none).AsGhost();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Ghost<TOut> Map<TOut>(Func<T, TOut> selector)
        {
            return Lang.Map(this, selector).AsGhost();
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
        public readonly T Value() => inner.GetValueOrDefault();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Value(T defaultValue)
        {
            return Lang.Value(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Value(Func<T> defaultValueFactory)
        {
            return Lang.Value(this, defaultValueFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T ValueUnsafe()
        {
            return Lang.ValueUnsafe<GhostStruct<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Ghost<TOut> Select<TOut>(Func<T, TOut> selector)
        {
            return Map(selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GhostStruct<T> Where(Predicate<T> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsNone || !predicate(inner.GetValueOrDefault()))
                return None;

            return this;
        }

        public readonly bool Equals(GhostStruct<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(inner, other.inner);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is GhostStruct<T> typed && Equals(typed);
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
