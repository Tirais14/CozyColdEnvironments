using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    [Serializable]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        partial struct Maybe<T> : IEquatable<Maybe<T>>
    {
        public readonly static Maybe<T> None = default;

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? target;

        [field: UnityEngine.SerializeField]
        public bool IsSome { get; private set; }
#else
        private readonly T? inner;

        public readonly bool IsSome { get; }
#endif

        public readonly bool IsNone => !IsSome;
        public readonly T? Raw => target;

        public Maybe(T value)
        {
            target = value;

            IsSome = value.IsNotDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Maybe<T>(T? source) => new(source!);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T?(Maybe<T> source) => source.target;

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Either<R>(R? right) => (target, right);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Catch() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> Resolve(Predicate<T>? predicate = null)
        {
            return (target, predicate);
        }

        public readonly bool Equals(Maybe<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(target, other.target);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Maybe<T> typed && Equals(typed);
        }
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(target);
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsNone)
                yield break;

            yield return target!;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
