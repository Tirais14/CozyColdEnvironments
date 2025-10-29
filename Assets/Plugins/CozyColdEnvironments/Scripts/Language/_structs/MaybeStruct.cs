using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
#pragma warning disable IDE1006
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    [Serializable]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        partial struct MaybeStruct<T> : IEquatable<MaybeStruct<T>>

        where T : struct
    {
        public static MaybeStruct<T> None => new();
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T inner;

        [UnityEngine.Range(int.MinValue, int.MaxValue)]
        [UnityEngine.SerializeField]
        private T defaultValue;
#else
        private readonly T inner;
        private readonly T defaultValue;
#endif

        public bool IsSome { get; private set; }
        public readonly bool IsNone => !IsSome;

        public MaybeStruct(T value)
            :
            this()
        {
            inner = value;
            IsSome = !inner.Equals(defaultValue);
        }

        public MaybeStruct(T value, T defaultValue)
            :
            this(value)
        {
            this.defaultValue = defaultValue;
        }

        public MaybeStruct(T value, bool hasValue)
            :
            this()
        {
            inner = value;
            IsSome = hasValue;
        }

        public MaybeStruct(T? value)
            :
            this()
        {
            IsSome = value.HasValue;
            inner = value.GetValueOrDefault();
        }

        public static implicit operator MaybeStruct<T>(T? source)
        {
            return new MaybeStruct<T>(source);
        }

        public static implicit operator Maybe<T>(MaybeStruct<T> source)
        {
            return source.inner;
        }

        public static explicit operator T(MaybeStruct<T> source)
        {
            return source.inner;
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

            yield return inner;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
