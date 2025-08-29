using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CozyColdEnvironments.Reflection.ObjectModel;

#nullable enable
namespace CozyColdEnvironments.Reflection.Cached
{
    public static class TypeCache
    {
        private readonly static Dictionary<Type, object?> defaultValuesCache = new();
        private readonly static Dictionary<ConstructrorKey, ConstructorInfo> ctorsCache = new();
        private readonly static Dictionary<FieldKey, FieldInfo> fieldsCache = new();
        private readonly static Dictionary<FieldKey, PropertyInfo> propsCache = new();

        public static bool TryCacheDefaultValue(Type type, object? value)
        {
            return defaultValuesCache.TryAdd(type, value);
        }

        public static bool TryCacheMember(MemberInfo member)
        {
            return member switch
            {
                ConstructorInfo ctor => ctorsCache.TryAdd(ConstructrorKey.Create(ctor), ctor),
                FieldInfo field => fieldsCache.TryAdd(FieldKey.Create(field), field),
                PropertyInfo prop => propsCache.TryAdd(FieldKey.Create(prop), prop),
                _ => throw new NotImplementedException(member.GetTypeName()),
            };
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetDefaultValue(Type type,
                                                   out object? defaultValue)
        {
            if (type is null)
                throw new ArgumentNullException("type");

            return defaultValuesCache.TryGetValue(type, out defaultValue);
        }

        public static bool TryGetConstructor(ConstructrorKey ctorKey,
            [NotNullWhen(true)] out ConstructorInfo? ctor)
        {
            return ctorsCache.TryGetValue(ctorKey, out ctor);
        }

        public static bool TryGetField(FieldKey fieldKey,
            [NotNullWhen(true)] out FieldInfo? field)
        {
            return fieldsCache.TryGetValue(fieldKey, out field);
        }

        public static bool TryGetProperty(FieldKey fieldKey,
            [NotNullWhen(true)] out PropertyInfo? prop)
        {
            return propsCache.TryGetValue(fieldKey, out prop);
        }

        public readonly struct ConstructrorKey : IEquatable<ConstructrorKey>
        {
            public readonly Type ReflectedType { get; }
            public readonly Type[] Signature { get; }
            public readonly ParameterModifier ParameterModifiers { get; }

            public ConstructrorKey(Type reflectedType,
                                   Type[] signature,
                                   ParameterModifier parameterModifiers)
            {
                ReflectedType = reflectedType;
                Signature = signature;
                ParameterModifiers = parameterModifiers;
            }
            public ConstructrorKey(Type reflectedType,
                                   Signature signature,
                                   ParameterModifier parameterModifiers)
                :
                this(reflectedType, (Type[])signature, parameterModifiers)
            {
            }

            public static ConstructrorKey Create(ConstructorInfo ctor)
            {
                ParameterInfo[] parameters = ctor.GetParameters();

                Type[] signature = parameters.Select(x => x.ParameterType).ToArray();
                ParameterModifier parameterModifiers = parameters.GetParameterModifiers();

                return new ConstructrorKey(ctor.ReflectedType,
                                           signature,
                                           parameterModifiers);
            }

            public bool Equals(ConstructrorKey other)
            {
                return other.ReflectedType == ReflectedType && other.Signature == Signature;
            }

            public override bool Equals(object? obj)
            {
                return obj is ConstructrorKey typed && Equals(typed);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ReflectedType, Signature);
            }
        }

        public readonly struct FieldKey : IEquatable<FieldKey>
        {
            public readonly Type ReflectedType { get; }
            public readonly (Type fieldType, string fieldName) ID { get; }

            public FieldKey(Type reflectedType, (Type fieldType, string fieldName) id)
            {
                ReflectedType = reflectedType;
                ID = id;
            }
            public FieldKey(Type reflectedType, Type fieldType)
                :
                this(reflectedType, (fieldType, string.Empty))
            {
            }

            public static FieldKey Create(FieldInfo field)
            {
                return new FieldKey(field.ReflectedType, (field.FieldType, field.Name));
            }
            public static FieldKey Create(PropertyInfo prop)
            {
                return new FieldKey(prop.ReflectedType, (prop.PropertyType, prop.Name));
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
}
