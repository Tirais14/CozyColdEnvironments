using System;
using System.Reflection;
using UTIRLib.Reflection;
using UTIRLib.Reflection.Cached;

#nullable enable
namespace UTIRLib
{
    public static class InstanceFactory
    {
        [Flags]
        public enum Parameters
        {
            None,
            CacheConstructor,
            ThrowIfNotFound = 2,
            Default = ThrowIfNotFound,
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static object Create(Type type,
                                    ConstructorBindings constructorParams,
                                    Parameters parameters = Parameters.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException($"Type {type.GetName()} is interface and not allowed to create.");
            if (constructorParams is null)
                throw new ArgumentNullException(nameof(constructorParams));

            bool throwIfNotFound = parameters.HasFlag(Parameters.ThrowIfNotFound);
            if (!TypeCache.TryGetConstructor(
                new TypeCache.ConstructrorKey(
                    type,
                    (Type[])constructorParams.Arguments,
                    constructorParams.ParameterModifiers),
                out ConstructorInfo? ctor))
            {
                ctor = type.GetConstructor(constructorParams, throwIfNotFound);
                if (parameters.HasFlag(Parameters.CacheConstructor))
                    TypeCache.TryCacheMember(ctor);
            }

            object?[] ctorArgs = (object?[])constructorParams.Arguments;
            return ctor.Invoke(ctorArgs);
        }

        public static T Create<T>(ConstructorBindings constructorParams,
            Parameters parameters = Parameters.Default)
        {
            return (T)Create(typeof(T), constructorParams, parameters);
        }
    }
}
