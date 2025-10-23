using System;
using System.Collections.Generic;

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

        public readonly T Value() => value;
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
