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
                                    bool cacheResults = false)
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

        public static object Create(Type type,
                                    InvokableArguments arguments,
                                    bool cacheResults = false)
        {
            return Create(type,
                          defaultConstructorParams with { ArgumentsData = arguments },
                          cacheResults);
        }

        public static T Create<T>(ConstructorParameters constructorParams,
                                  bool cacheResults = false)
        {
            return (T)Create(typeof(T), constructorParams, cacheResults);
        }

        public static T Create<T>(InvokableArguments arguments, bool cacheResults = false)
        {
            return Create<T>(
                defaultConstructorParams with { ArgumentsData = arguments },
                cacheResults);
        }

        public static bool TryCreate(Type type,
                                     ConstructorParameters constructorParameters,
                                     [NotNullWhen(true)] out object? result,
                                     bool cacheResult = false)
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

        public static bool TryCreate(Type type,
                                       InvokableArguments arguments,
                                       [NotNullWhen(true)] out object? result,
                                       bool cacheResult = false)
        {
            return TryCreate(type,
                             defaultConstructorParams with { ArgumentsData = arguments },
                             out result,
                             cacheResult);
        }

        public static bool TryCreate<T>(ConstructorParameters constructorParameters,
                                        [NotNullWhen(true)] out T? result,
                                        bool cacheResult = false)
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

        public static bool TryCreate<T>(InvokableArguments arguments,
                                        [NotNullWhen(true)] out T? result,
                                        bool cacheResult = false)
        {
            return TryCreate(defaultConstructorParams with { ArgumentsData = arguments },
                             out result,
                             cacheResult);
        }
    }
}
