using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Cacheables
{
    public readonly struct FieldKey : IEquatable<FieldKey>
    {
        public readonly Type ReflectedType { get; }
        public readonly (Type? fieldType, string fieldName) ID { get; }

        public FieldKey(Type reflectedType, string fieldName, Type? fieldType = null)
        {
            ReflectedType = reflectedType;
            ID = (fieldType, fieldName);
        }
        public FieldKey(Type reflectedType, Type fieldType)
            :
            this(reflectedType, string.Empty, fieldType)
        {
        }

        public static FieldKey Create(FieldInfo field)
        {
            return new FieldKey(field.ReflectedType, field.Name, field.FieldType);
        }
        public static FieldKey Create(PropertyInfo prop)
        {
            return new FieldKey(prop.ReflectedType, prop.Name, prop.PropertyType);
        }

        public bool Equals(FieldKey other)
        {
            return other.ReflectedType == ReflectedType
                   &&
                   other.ID.Equals(ID);
        }

        public override bool Equals(object obj)
        {
            return obj is FieldKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReflectedType, ID);
        }
    }
}
