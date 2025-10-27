using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable IDE1006
#pragma warning disable S3236
namespace CCEnvs.Language
{
    [Serializable]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        partial struct MaybeStruct<T>
        : IEquatable<MaybeStruct<T>>,
        IConditional<MaybeStruct<T>, T>

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

        public static implicit operator Maybe<T>(MaybeStruct<T> source)
        {
            return source.inner.GetValueOrDefault();
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
