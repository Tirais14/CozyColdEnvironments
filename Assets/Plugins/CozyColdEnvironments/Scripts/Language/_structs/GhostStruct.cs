using System;
using System.Collections;
using System.Collections.Generic;

namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct GhostStruct<T>
        : IEnumerable<T>,
        IEquatable<GhostStruct<T>>,
        IConditional

        where T : struct
    {
        public static GhostStruct<T> None => new();

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? inner;
#else
        private readonly T? inner;
#endif

        public readonly bool IsSome => inner.HasValue;
        public readonly bool IsNone => !IsSome;

        public GhostStruct(T value)
        {
            this.inner = value;
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

        public readonly T? Value() => inner;

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

            yield return inner.Value;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
