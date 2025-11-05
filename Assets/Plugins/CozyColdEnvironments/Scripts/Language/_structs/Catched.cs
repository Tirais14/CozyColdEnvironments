using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static UnityEngine.GraphicsBuffer;

#nullable enable
#pragma warning disable S3236
#pragma warning disable IDE1006
namespace CCEnvs.FuncLanguage
{
    [Serializable]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        partial struct Catched<T> : IEquatable<Catched<T>>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T? target;

        [field: UnityEngine.SerializeField]
        public bool IsSome { get; private set; }

        [field: UnityEngine.SerializeField]
        public LogType logType { get; private set; }
#else
        private readonly T? inner;

        public readonly bool IsSome { get; }

        public LogType logType { get; }
#endif

        public readonly bool IsNone => !IsSome;
        public readonly T? Raw => target;

        public Catched(T? value, LogType logType = LogType.Log)
            :
            this()
        {
            target = value;
            this.logType = logType;

            IsSome = value.IsNotDefault();
        }

        public Catched(Func<T> valueFactory, LogType logType = LogType.Log)
            :
            this()
        {
            try
            {
                target = valueFactory();
                this.logType = logType;

                IsSome = target.IsNotDefault();
            }
            catch (Exception ex)
            {
                this.PrintDebug(ex, logType);
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

        public static implicit operator Catched<T>(T? source)
        {
            return new Catched<T>(source);
        }

        public static explicit operator T?(Catched<T> source)
        {
            return source.target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> With(LogType logType)
        {
            return new Catched<T>(target, logType);
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

        public readonly bool Equals(Catched<T> other)
        {
            return EqualityComparer<T?>.Default.Equals(target, other.target);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Catched<T> typed && Equals(typed);
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
