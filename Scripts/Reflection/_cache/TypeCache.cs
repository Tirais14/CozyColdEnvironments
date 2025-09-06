using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection.Data;

#nullable enable
namespace CCEnvs.Reflection.Cached
{
    public static class TypeCache
    {
        private readonly static Dictionary<Type, object?> defaultValuesCache = new();
        private readonly static Dictionary<MethodKey, ConstructorInfo> ctorsCache = new();
        private readonly static Dictionary<MethodKey, MethodInfo> methodsCache = new();
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
                ConstructorInfo ctor => ctorsCache.TryAdd(MethodKey.Create(ctor), ctor),
                MethodInfo method => methodsCache.TryAdd(MethodKey.Create(method), method),
                FieldInfo field => fieldsCache.TryAdd(FieldKey.Create(field), field),
                PropertyInfo prop => propsCache.TryAdd(FieldKey.Create(prop), prop),
                _ => throw new NotImplementedException(member.GetTypeName()),
            };
        }

        public static bool UncacheMember(MemberInfo member)
        {
            return member switch
            {
                ConstructorInfo ctor => ctorsCache.Remove(MethodKey.Create(ctor)),
                MethodInfo method => methodsCache.Remove(MethodKey.Create(method)),
                FieldInfo field => fieldsCache.Remove(FieldKey.Create(field)),
                PropertyInfo prop => propsCache.Remove(FieldKey.Create(prop)),
                _ => throw new NotImplementedException(member.GetTypeName()),
            };
        }

        public static void Clear()
        {
            defaultValuesCache.Clear();
            ctorsCache.Clear();
            methodsCache.Clear();
            fieldsCache.Clear();
            propsCache.Clear();

            TrimExcess();
        }

        public static void TrimExcess()
        {
            defaultValuesCache.TrimExcess();
            ctorsCache.TrimExcess();
            methodsCache.TrimExcess();
            fieldsCache.TrimExcess();
            propsCache.TrimExcess();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetDefaultValue(Type type,
                                              out object? defaultValue)
        {
            if (type is null)
                throw new ArgumentNullException("type");

            return defaultValuesCache.TryGetValue(type, out defaultValue);
        }

        public static bool TryGetConstructor(MethodKey ctorKey,
            [NotNullWhen(true)] out ConstructorInfo? ctor)
        {
            return ctorsCache.TryGetValue(ctorKey, out ctor);
        }

        public static bool TryGetMethod(MethodKey methodKey,
            [NotNullWhen(true)] out MethodInfo? method)
        {
            return methodsCache.TryGetValue(methodKey, out method);
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

        public readonly struct MethodKey : IEquatable<MethodKey>
        {
            public readonly Type ReflectedType { get; }
            public readonly Type[] ParameterTypes { get; }
            public readonly ParameterModifier ParameterModifiers { get; }

            public MethodKey(Type reflectedType,
                                   Type[] parameterTypes,
                                   ParameterModifier parameterModifiers)
            {
                ReflectedType = reflectedType;
                ParameterTypes = parameterTypes;
                ParameterModifiers = parameterModifiers;
            }

            public MethodKey(Type reflectedType,
                                   CCParameters parameters,
                                   ParameterModifier parameterModifiers)
                :
                this(reflectedType, (Type[])parameters, parameterModifiers)
            {
            }

            public static MethodKey Create(MethodBase method)
            {
                Validate.ArgumentNull(method, nameof(method));

                ParameterInfo[] parameters = method.GetParameters();

                Type[] signature = parameters.Select(x => x.ParameterType).ToArray();
                ParameterModifier parameterModifiers = parameters.GetParameterModifiers();

                return new MethodKey(method.ReflectedType,
                                           signature,
                                           parameterModifiers);
            }

            public bool Equals(MethodKey other)
            {
                return ReflectedType == other.ReflectedType && ParameterTypes.SequenceEqual(other.ParameterTypes);
            }

            public override bool Equals(object? obj)
            {
                return obj is MethodKey typed && Equals(typed);
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(ReflectedType);

                int count = ParameterTypes.Length;
                for (int i = 0; i < count; i++)
                    hash.Add(ParameterTypes[i]);

                return hash.ToHashCode();
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
