using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct Catched<T>
        : IEnumerable<T>,
        IEquatable<Catched<T>>,
        ITarget<Catched<T>, T>
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
        public readonly Catched<TOut> Map<TOut>(Func<T, TOut> selector)
        {
            return Lang.TryMap(this, selector, logType).Catch();
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
        public readonly Catched<TOut> Match<TOut>(Func<T, TOut> some, Func<TOut> none)
        {
            return Lang.TryMatch(this, some, none, logType).Catch();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access() => inner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Access([NotNullWhen(true)] out T? result)
        {
            result = inner;

            return IsSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access(T? defaultValue)
        {
            return Lang.Access(this, defaultValue);
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
