using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.Cached;
using UTIRLib.Reflection.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class InstanceFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static object Create(Type type,
                                    ConstructorParameters constructorParams,
                                    bool cacheResults)
        {
            if (constructorParams is null)
                throw new ArgumentNullException(nameof(constructorParams));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            ConstructorInfo? ctor;
            if (cacheResults)
                ctor = TypeCache.GetConstructor(type, constructorParams);
            else
            {
                ctor = type.GetConstructor(constructorParams.BindingFlags,
                                           constructorParams.Binder,
                                           constructorParams.CallingConvention,
                                           constructorParams.Signature,
                                           constructorParams.ParameterModifiers)
                    ??
                    throw new MemberNotFoundException(type,
                                                      MemberType.Constructor,
                                                      constructorParams);
            }

            object?[] ctorArgs = constructorParams.Arguments;
            return ctor.Invoke(ctorArgs);
        }

        public static T Create<T>(ConstructorParameters constructorParams,
                                  bool cacheResults)
        {
            return (T)Create(typeof(T), constructorParams, cacheResults);
        }

        public static bool TryCreate(Type type,
                                     ConstructorParameters constructorParameters,
                                     bool cacheResult,
                                     [NotNullWhen(true)] out object? result)
        {
            try
            {
                result = Create(type, constructorParameters, cacheResult);

                return result is not null;
            }
            catch (MemberNotFoundException)
            {
                result = null;

                return false;
            }
        }

        public static bool TryCreate<T>(ConstructorParameters constructorParameters,
                                        bool cacheResult,
                                        [NotNullWhen(true)] out T? result)
        {
            try
            {
                result = Create<T>(constructorParameters, cacheResult);

                return result is not null;
            }
            catch (MemberNotFoundException)
            {
                result = default;

                return false;
            }
        }
    }
}
