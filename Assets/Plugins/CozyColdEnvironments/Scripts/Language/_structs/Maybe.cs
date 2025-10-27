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
        partial struct Maybe<T>
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
