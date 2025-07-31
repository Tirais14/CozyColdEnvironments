using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.Cached;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class InstanceFactory
    {
        private readonly static ConstructorParameters defaultConstructorParams = new()
        { 
            BindingFlags = BindingFlagsDefault.InstancePublic
        };

        /// <exception cref="ArgumentNullException"></exception>
        public static object Create(Type type,
                                    ConstructorParameters constructorParams,
                                    bool cacheConstructor = false)
        {
            if (constructorParams is null)
                throw new ArgumentNullException(nameof(constructorParams));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            ConstructorInfo? ctor;
            if (cacheConstructor)
                ctor = TypeCache.GetConstructor(type, constructorParams);
            else
                ctor = type.GetConstructor(constructorParams, throwIfNotFound: true);

            object?[] ctorArgs = constructorParams.Arguments;
            return ctor.Invoke(ctorArgs);
        }

        public static object Create(Type type,
                                    InvokableArguments arguments,
                                    bool cacheConstructor = false)
        {
            return Create(type,
                          defaultConstructorParams with { ArgumentsData = arguments },
                          cacheConstructor);
        }

        public static T Create<T>(ConstructorParameters constructorParams,
                                  bool cacheConstructor = false)
        {
            return (T)Create(typeof(T), constructorParams, cacheConstructor);
        }

        public static T Create<T>(InvokableArguments arguments, bool cacheConstructor = false)
        {
            return Create<T>(
                defaultConstructorParams with { ArgumentsData = arguments },
                cacheConstructor);
        }

        public static bool TryCreate(Type type,
                                     ConstructorParameters constructorParameters,
                                     [NotNullWhen(true)] out object? result,
                                     bool cacheConstructor = false)
        {
            try
            {
                result = Create(type, constructorParameters, cacheConstructor);

                return result is not null;
            }
            catch (MemberNotFoundException)
            {
                result = null;

                return false;
            }
        }

        public static bool TryCreate(Type type,
                                       InvokableArguments arguments,
                                       [NotNullWhen(true)] out object? result,
                                       bool cacheConstructor = false)
        {
            return TryCreate(type,
                             defaultConstructorParams with { ArgumentsData = arguments },
                             out result,
                             cacheConstructor);
        }

        public static bool TryCreate<T>(ConstructorParameters constructorParameters,
                                        [NotNullWhen(true)] out T? result,
                                        bool cacheConstructor = false)
        {
            try
            {
                result = Create<T>(constructorParameters, cacheConstructor);

                return result is not null;
            }
            catch (MemberNotFoundException)
            {
                result = default;

                return false;
            }
        }

        public static bool TryCreate<T>(InvokableArguments arguments,
                                        [NotNullWhen(true)] out T? result,
                                        bool cacheConstructor = false)
        {
            return TryCreate(defaultConstructorParams with { ArgumentsData = arguments },
                             out result,
                             cacheConstructor);
        }
    }
}
