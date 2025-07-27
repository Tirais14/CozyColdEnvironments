using System;
using System.Collections.Generic;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection.Cached
{
    public static class TypeCache
    {
        private readonly static Dictionary<Type, object> defaultValuesCollection = new();

        private readonly static Dictionary<ConstructrorKey, ConstructorInfo> constructorsCollection = new();

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

            if (constructorsCollection.TryGetValue(new ConstructrorKey(type,
                        constructorParams.ArgumentsData.Signature),
                    out ConstructorInfo constructor)
                )
                return constructor;

            constructor = type.GetConstructor(constructorParams.BindingFlags,
                                              constructorParams.Binder,
                                              constructorParams.CallingConvention,
                                              constructorParams.Signature,
                                              constructorParams.ParameterModifiers) 
                ??
                throw new MemberNotFoundException(type, MemberType.Constructor, constructorParams);

            constructorsCollection.Add(new ConstructrorKey(type,
                    constructorParams.ArgumentsData.Signature),
                constructor);

            return constructor;
        }
    }
}
