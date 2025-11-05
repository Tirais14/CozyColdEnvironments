using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        private T target;

        [UnityEngine.SerializeField]
        [UnityEngine.Range(int.MinValue, int.MaxValue)]
        private T defaultValue;

        [field: UnityEngine.SerializeField]
        public bool IsSome { get; private set; }
#else
        private readonly T inner;
        private readonly T defaultValue;

        public bool IsSome { get; }
#endif

        public readonly bool IsNone => !IsSome;

        public readonly T Raw => target;

        public MaybeStruct(T value)
            :
            this()
        {
            this.target = value;
            IsSome = !this.target.Equals(defaultValue);
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
            this.target = value;
            IsSome = hasValue;
        }

        public MaybeStruct(T? value)
            :
            this()
        {
            IsSome = value.HasValue;
            this.target = value.GetValueOrDefault();
        }

        public static implicit operator MaybeStruct<T>(T? source)
        {
            return new MaybeStruct<T>(source);
        }

        public static implicit operator MaybeStruct<T>((T value, bool hasValue) source)
        {
            return new MaybeStruct<T>(source.value, source.hasValue);
        }

        public static implicit operator MaybeStruct<T>((T value, Predicate<T> predicate) source)
        {
            return new MaybeStruct<T>(source.value, source.predicate(source.value));
        }

        public static implicit operator Maybe<T>(MaybeStruct<T> source)
        {
            return source.target;
        }

        public static explicit operator T(MaybeStruct<T> source)
        {
            return source.target;
        }

        public static bool operator ==(MaybeStruct<T> left, MaybeStruct<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaybeStruct<T> left, MaybeStruct<T> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Either<R>(R? right) => (target, right);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> Resolve(Predicate<T>? predicate = null)
        {
            return (target, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Maybe() => target;

        public readonly bool Equals(MaybeStruct<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(target, other.target);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is MaybeStruct<T> typed && Equals(typed);
        }
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(target, IsSome);
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsNone)
                yield break;

            yield return target;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
