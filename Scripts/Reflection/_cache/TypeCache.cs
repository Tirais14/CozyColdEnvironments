using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection.Cached
{
    public static class TypeCache
    {
        private readonly static Dictionary<Type, object> defaultValuesCollection = new();

        private readonly static Dictionary<ConstructrorKey, ConstructorInfo> constructorsCache = new();

        private readonly static Dictionary<FieldKey, FieldInfo> fieldsCache = new();

        public static object? GetDefaultValue(Type type)
        {
            if (type.IsClass)
                return null;

            if (defaultValuesCollection.TryGetValue(type, out object defaultValue))
                return defaultValue;

            defaultValue = Activator.CreateInstance(type);
            defaultValuesCollection.Add(type, defaultValue);

            return defaultValue;
        }

        public static ConstructorInfo GetConstructor(Type type,
                                                     ConstructorParameters constructorParams)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (constructorParams == null)
                throw new ArgumentNullException(nameof(constructorParams));

            if (constructorsCache.TryGetValue(new ConstructrorKey(type,
                        constructorParams.ArgumentsData.Signature),
                    out ConstructorInfo constructor)
                )
                return constructor;

            constructor = type.GetConstructor(constructorParams, throwIfNotFound: true);

            constructorsCache.Add(new ConstructrorKey(type,
                    constructorParams.ArgumentsData.Signature),
                constructor);

            return constructor;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsConstructorCached(Type type,
                                               InvokableSignature signature)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var constructrorKey = new ConstructrorKey(type, signature);

            return constructorsCache.ContainsKey(constructrorKey);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        public static FieldInfo GetField(Type type,
                                         string name,
                                         BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic,
                                         bool throwIfNotFound = true)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name.IsNullOrEmpty())
                throw new StringArgumentException(nameof(name), name);

            var key = new FieldKey(type, name);

            if (fieldsCache.TryGetValue(key, out FieldInfo? field))
                return field;

            field = type.GetField(name, bindingFlags);

            if (field is null)
            {
                if (throwIfNotFound)
                    throw new MemberNotFoundException(type, MemberType.Field, name);
                else
                    return null!;
            }

            fieldsCache.Add(key, field);

            return field;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo GetField(Type type,
                                         Type fieldType,
                                         BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic,
                                         bool throwIfNotFound = true)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (fieldType is null)
                throw new ArgumentNullException(nameof(fieldType));


            var key = new FieldKey(type, fieldType);

            if (fieldsCache.TryGetValue(key, out FieldInfo? field))
                return field;

            field = type.GetField(fieldType, bindingFlags);

            if (field is null)
            {
                if (throwIfNotFound)
                    throw new MemberNotFoundException(type, MemberType.Field);
                else
                    return null!;
            }

            fieldsCache.Add(key, field);

            return field;
        }

        public readonly struct ConstructrorKey : IEquatable<ConstructrorKey>
        {
            public readonly Type ReflectedType { get; }
            public readonly InvokableSignature Signature { get; }

            public ConstructrorKey(Type type, InvokableSignature signature)
            {
                ReflectedType = type;
                Signature = signature;
            }

            public bool Equals(ConstructrorKey other)
            {
                return other.ReflectedType == ReflectedType && other.Signature == Signature;
            }

            public override bool Equals(object? obj)
            {
                return obj is InvokableArguments typed && Equals(typed);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ReflectedType, Signature);
            }
        }

        public readonly struct FieldKey : IEquatable<FieldKey>
        {
            public readonly Type ReflectedType { get; }

            public readonly string? Name { get; }
            public readonly Type? FieldType { get; }

            public FieldKey(Type reflectedType, string name)
            {
                ReflectedType = reflectedType;
                Name = name;
                FieldType = null;
            }

            public FieldKey(Type reflectedType, Type fieldType)
            {
                ReflectedType = reflectedType;
                FieldType = fieldType;
                Name = null;
            }

            public bool Equals(FieldKey other)
            {
                return other.ReflectedType == ReflectedType 
                       &&
                       other.Name == Name
                       &&
                       other.FieldType == FieldType;
            }

            public override bool Equals(object obj)
            {
                return obj is FieldKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ReflectedType, Name, FieldType);
            }
        }
    }
}
