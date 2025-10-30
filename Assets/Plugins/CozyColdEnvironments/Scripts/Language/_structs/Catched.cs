using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private T? inner;

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

        public Catched(T? value, LogType logType = LogType.Log)
            :
            this()
        {
            inner = value;
            this.logType = logType;

            IsSome = value.IsNotDefault();
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

        public static implicit operator Catched<T>(T? source)
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
