using System;
using System.Reflection;
using UTIRLib.Reflection;
using UTIRLib.Reflection.Cached;

#nullable enable
namespace UTIRLib
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
                                    InstanceCreationParameters parameters = InstanceCreationParameters.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException($"Type {type.GetName()} is interface and not allowed to create.");
            if (constructorParams is null)
                throw new ArgumentNullException(nameof(constructorParams));

            ConstructorInfo? ctor;
            if (TypeCache.IsConstructorCached(
                type,
                constructorParams.Signature
                )
                ||
                parameters.IsFlagSetted(InstanceCreationParameters.CacheConstructor)
                )
                ctor = TypeCache.GetConstructor(type, constructorParams);
            else
                ctor = type.GetConstructor(constructorParams, throwIfNotFound: true);

            object?[] ctorArgs = constructorParams.Arguments;
            return ctor.Invoke(ctorArgs);
        }

        public static object Create(Type type,
                                    InvokableArguments arguments,
                                    InstanceCreationParameters parameters = InstanceCreationParameters.Default)
        {
            return Create(type,
                          defaultConstructorParams with { ArgumentsData = arguments },
                          parameters);
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static object Create(Type type,
                                    InstanceCreationParameters parameters,
                                    params object[] args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            return Create(type, new InvokableArguments(args), parameters);
        }
        public static object Create(Type type, params object[] args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            return Create(type,
                          new InvokableArguments(args),
                          parameters: InstanceCreationParameters.Default);
        }

        public static T Create<T>(ConstructorParameters constructorParams,
                                  InstanceCreationParameters parameters = InstanceCreationParameters.Default)
        {
            return (T)Create(typeof(T), constructorParams, parameters);
        }
        public static T Create<T>(InvokableArguments arguments,
                                  InstanceCreationParameters parameters = InstanceCreationParameters.Default)
        {
            return Create<T>(
                defaultConstructorParams with { ArgumentsData = arguments },
                parameters);
        }
        public static T Create<T>(InstanceCreationParameters parameters,
                                  params object[] args)
        {
            return (T)Create(typeof(T),
                             parameters,
                             args);
        }
        public static T Create<T>(params object[] args)
        {
            return (T)Create(typeof(T),
                             parameters: InstanceCreationParameters.Default,
                             args);
        }

        //public static bool TryCreate(Type type,
        //                             ConstructorParameters constructorParameters,
        //                             [NotNullWhen(true)] out object? result,
        //                             bool cacheConstructor = false)
        //{
        //    try
        //    {
        //        result = Create(type, constructorParameters, cacheConstructor);

        //        return result is not null;
        //    }
        //    catch (MemberNotFoundException)
        //    {
        //        result = null;

        //        return false;
        //    }
        //}
        //public static bool TryCreate(Type type,
        //                               InvokableArguments arguments,
        //                               [NotNullWhen(true)] out object? result,
        //                               bool cacheConstructor = false)
        //{
        //    return TryCreate(type,
        //                     defaultConstructorParams with { ArgumentsData = arguments },
        //                     out result,
        //                     cacheConstructor);
        //}
        //public static bool TryCreate(Type type,
        //                             bool cacheConstructor,
        //                             out object? result,
        //                             params object[] args)
        //{
        //    return TryCreate(type,
        //                     new InvokableArguments(args),
        //                     out result,
        //                     cacheConstructor);
        //}
        //public static bool TryCreate(Type type,
        //                             out object? result,
        //                             params object[] args)
        //{
        //    return TryCreate(type,
        //                     cacheConstructor: false,
        //                     out result,
        //                     args);
        //}

        //public static bool TryCreate<T>(ConstructorParameters constructorParameters,
        //                                [NotNullWhen(true)] out T? result,
        //                                bool cacheConstructor = false)
        //{
        //    try
        //    {
        //        result = Create<T>(constructorParameters, cacheConstructor);

        //        return result is not null;
        //    }
        //    catch (MemberNotFoundException)
        //    {
        //        result = default;

        //        return false;
        //    }
        //}
        //public static bool TryCreate<T>(InvokableArguments arguments,
        //                                [NotNullWhen(true)] out T? result,
        //                                bool cacheConstructor = false)
        //{
        //    return TryCreate(defaultConstructorParams with { ArgumentsData = arguments },
        //                     out result,
        //                     cacheConstructor);
        //}
        //public static bool TryCreate<T>(bool cacheConstructor,
        //                                out T? result,
        //                                params object[] args)
        //{
        //    bool state = TryCreate(typeof(T),
        //                           cacheConstructor,
        //                           out object? resultNonTyped,
        //                           args);

        //    result = (T?)resultNonTyped;
        //    return state;
        //}
        //public static bool TryCreate<T>(out T? result,
        //                                params object[] args)
        //{
        //    bool state = TryCreate(typeof(T),
        //                           cacheConstructor: false,
        //                           out object? resultNonTyped,
        //                           args);

        //    result = (T?)resultNonTyped;
        //    return state;
        //}
    }
}
