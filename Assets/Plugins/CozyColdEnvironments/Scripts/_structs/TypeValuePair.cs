using CCEnvs.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    [Serializable]
    public struct TypeValuePair : IEquatable<TypeValuePair>
    {
        public static TypeValuePair Empty => T<object>();

        [JsonProperty]
        public Type Type { get; private set; }

        [JsonProperty]
        public object? Value { get; private set; }

        [JsonConstructor]
        public TypeValuePair(Type type, object? value)
        {
            Type = type;
            Value = value;
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TypeValuePair((Type type, object? value) input)
        {
            return new TypeValuePair(input.type, input.value);
        }

        public static TypeValuePair T<T>()
        {
            return new TypeValuePair(typeof(T));
        }
        public static TypeValuePair T<T>(T? value)
        {
            return new TypeValuePair(typeof(T), value);
        }

        public static bool operator ==(TypeValuePair left, TypeValuePair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeValuePair left, TypeValuePair right)
        {
            return !left.Equals(right);
        }

        public readonly bool Equals(TypeValuePair other)
        {
            return Type == other.Type && Value == other.Value;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is TypeValuePair typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }

        public readonly override string ToString()
        {
            return Type.GetName();
        }
    }
}
