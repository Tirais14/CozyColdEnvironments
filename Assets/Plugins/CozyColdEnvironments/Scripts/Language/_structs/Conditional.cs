using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Language
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly
#endif
        struct Conditional<T> : IEquatable<Conditional<T>>, IConditional<T>
    {

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
        private T value;
#else
        private readonly T value;
#endif
        public readonly bool IsSome => value.IsNotDefault();
        public readonly bool IsNone => !IsSome;

        public Conditional(T value)
        {
            this.value = value; 
        }

        public static bool operator ==(Conditional<T> left, Conditional<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Conditional<T> left, Conditional<T> right)
        {
            return !(left == right);
        }

        public static implicit operator Conditional<T>(T source)
        {
            return new Conditional<T>(source);  
        }

        public static implicit operator T(Conditional<T> source)
        {
            return source.value;
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
        public readonly T? Value() => value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Value([NotNullWhen(true)] out T? result)
        {
            result = value;

            return IsSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Value(T? defaultValue)
        {
            return Lang.Value(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Value(Func<T?> defaultValueFactory)
        {
            return Lang.Value(this, defaultValueFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T ValueUnsafe()
        {
            return Lang.ValueUnsafe<Conditional<T>, T>(this);
        }

        public readonly bool Equals(Conditional<T> other)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Conditional<T> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}
