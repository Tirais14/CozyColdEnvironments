using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Language
{
    [Serializable]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct Catched<T>
        : IEnumerable<T>,
        IEquatable<Catched<T>>,
        IConditional<Catched<T>, T>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? inner;
#else
        private readonly T? inner;
#endif

        public LogType logType { get; }

        public readonly bool IsSome => inner.IsNotDefault();
        public readonly bool IsNone => !IsSome;

        public Catched(T? value, LogType logType = LogType.Log)
            :
            this()
        {
            inner = value;
            this.logType = logType;
        }

        public Catched(Func<T> valueFactory)
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

        public static bool operator ==(Catched<T> left, Catched<T> right)
        {
            return left.Equals(right);
        }

        public static implicit operator Maybe<T>(Catched<T> source)
        {
            return source.inner!;
        }

        public static bool operator !=(Catched<T> left, Catched<T> right)
        {
            return !(left == right);
        }

        public static implicit operator Catched<T>(T source)
        {
            return new Catched<T>(source);
        }

        public static explicit operator T?(Catched<T> source)
        {
            return source.inner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> With(LogType logType)
        {
            return new Catched<T>(inner, logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> IfSome(Action<T> action)
        {
            return Lang.TryIfSome(this, action, logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Match(Action<T> some, Action none)
        {
            return Lang.TryMatch(this, some, none, logType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none)
        {
            return Lang.TryMatch(this, some, none, logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut?> selector)
        {
            return Lang.TryMap(this, selector, logType);
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
            return Lang.AccessUnsafe<Catched<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Apply(T? value)
        {
            return value!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Select<TOut>(Func<T, TOut?> selector)
        {
            return Map(selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Where(Predicate<T> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsNone || !predicate(inner!))
                return default!;

            return this;
        }

        public readonly bool Equals(Catched<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(inner, other.inner);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Catched<T> typed && Equals(typed);
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
