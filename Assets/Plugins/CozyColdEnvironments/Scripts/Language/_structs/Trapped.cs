using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct Trapped<T>
        : IEnumerable<T>,
        IEquatable<Trapped<T>>,
        ITarget<Trapped<T>, T>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? inner;
#else
        private readonly T? inner;
#endif

        public readonly bool IsSome => inner.IsNotDefault();
        public readonly bool IsNone => !IsSome;

        public Trapped(T value)
            :
            this()
        {
            inner = value;
        }

        public Trapped(Func<T> valueFactory)
            :
            this()
        {
            try
            {
                inner = valueFactory();
            }
            catch (Exception ex)
            {
                this.PrintLog(ex);
            }
        }

        public static bool operator ==(Trapped<T> left, Trapped<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Trapped<T> left, Trapped<T> right)
        {
            return !(left == right);
        }

        public static implicit operator Trapped<T>(T source)
        {
            return new Trapped<T>(source);
        }

        public static explicit operator T?(Trapped<T> source)
        {
            return source.inner;
        }

        public readonly Trapped<TOut> Map<TOut>(Func<T, TOut> selector)
        {
            return Lang.TryMap(this, selector).AsTrapped();
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
        public readonly Trapped<T> IfSome(Action<T> action)
        {
            return Lang.TryIfSome(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Trapped<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Trapped<T> Match(Action<T> some, Action none)
        {
            return Lang.TryMatch(this, some, none);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Trapped<TOut> Match<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return Lang.TryMatch(this, some, none).AsTrapped();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Value() => inner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Value(T? defaultValue)
        {
            return Lang.Value(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Value(Func<T?> defaultValueFactory)
        {
            return Lang.Value(this, defaultValueFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T ValueUnsafe()
        {
            return Lang.ValueUnsafe<Trapped<T>, T>(this);
        }

        public readonly bool Equals(Trapped<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(inner, other.inner);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Trapped<T> typed && Equals(typed);
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
