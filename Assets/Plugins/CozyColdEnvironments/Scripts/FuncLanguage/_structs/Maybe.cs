using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using static CCEnvs.FuncLanguage.LangOperator;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    [Serializable]
    [TypeSerializationDescriptor("FuncLanguage.Maybe<>", "5e3a47b4-5306-4c87-b8dd-41da28cbdd13")]
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        partial struct Maybe<T> : IEquatable<Maybe<T>>
    {
        public readonly static Maybe<T> None = default;
        //private static readonly Lazy<bool> targetIsStruct = new((() => typeof(T).IsValueType));

#if UNITY_2017_1_OR_NEWER
        [JsonProperty("target")]
        [UnityEngine.SerializeField]
        private T? target;

        [JsonProperty("default")]
        [UnityEngine.SerializeField]
        [UnityEngine.Tooltip("If target == default value marked as none.")]
        private T? @default;

        [JsonProperty("isSome")]
        public bool IsSome { get; private set; }
#else
        [JsonProperty("target")]
        private readonly T? target;

        [JsonProperty("target")]
        private readonly T? @default;

        public readonly bool IsSome { get; }
#endif

        public readonly bool IsNone => !IsSome;

        public readonly T? Raw => target;

        public Maybe(T? value)
        {
            target = value;
            @default = default;

            IsSome = IsSome(value);
        }

        public Maybe(T? value, T? @default)
        {
            target = value;
            this.@default = @default;

            IsSome = IsSome(value, @default);
        }

        public Maybe(T? value, Predicate<T?> isSome)
            :
            this()
        {
            Guard.IsNotNull(isSome, nameof(isSome));

            target = value;
            IsSome = isSome(value);
        }

        [JsonConstructor]
        private Maybe(T? value, T? @default = default, bool? isSome = null)
        {
            target = value;
            this.@default = @default;

            IsSome = isSome ?? IsSome(value, @default);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Maybe<T>(T? source) => new(source!);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Maybe<T>((T? value, T? @default) input)
        {
            return new Maybe<T>(input.value, input.@default);
        }

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
        public readonly IfElse<T> Resolve(Predicate<T>? predicate = null)
        {
            return (target, predicate);
        }

        public readonly bool Equals(Maybe<T> other)
        {
            var comparer = EqualityComparer<T?>.Default;

            return comparer.Equals(target, other.target)
                   &&
                   comparer.Equals(@default, other.@default)
                   &&
                   IsSome == other.IsSome;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Maybe<T> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(target, @default, IsSome);
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"({target}; {nameof(IsSome)}: {IsSome})";
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
