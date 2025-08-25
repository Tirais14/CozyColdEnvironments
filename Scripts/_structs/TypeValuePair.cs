using System;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib
{
    public readonly struct TypeValuePair : IEquatable<TypeValuePair>
    {
        public readonly Type type;
        public readonly object? value;

        public TypeValuePair(Type type, object? value)
        {
            this.type = type;
            this.value = value;
        }

        public TypeValuePair(object value)
            :
            this(value.GetType(), value)
        {
        }
        public TypeValuePair(Type type)
            :
            this(type, value: null)
        {
        }

        public static bool operator ==(TypeValuePair left, TypeValuePair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeValuePair left, TypeValuePair right)
        {
            return !left.Equals(right);
        }

        public bool Equals(TypeValuePair other)
        {
            return type == other.type && value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeValuePair typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, value);
        }

        public override string ToString()
        {
            return type.GetName();
        }
    }
}
