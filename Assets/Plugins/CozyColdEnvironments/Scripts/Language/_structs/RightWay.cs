using System;
using System.Collections.Generic;

namespace CCEnvs.Language
{
    public readonly struct RightWay<T>
    {
        public readonly Ghost<T> value;

        public RightWay(T value)
        {
            this.value = value;
        }

        public static bool operator ==(RightWay<T> left, RightWay<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RightWay<T> left, RightWay<T> right)
        {
            return !(left == right);
        }

        public static implicit operator RightWay<T>(T source)
        {
            return new RightWay<T>(source);
        }

        public static implicit operator T(RightWay<T> source)
        {
            return source.value.Value();
        }

        public bool Equals(RightWay<T> other)
        {
            return value == other.value;
        }
        public override bool Equals(object obj)
        {
            return obj is LeftWay<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }
    }
}
