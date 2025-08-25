using System;
using System.Reflection;
using UTIRLib.Reflection;
using UTIRLib.Reflection.Cached;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.ObjectModel;

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
                                    ConstructorBindings bindings,
                                    Parameters parameters = Parameters.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException($"Type {type.GetName()} is interface and not allowed to create.");
            if (bindings is null)
                throw new ArgumentNullException(nameof(bindings));

            bool throwIfNotFound = parameters.HasFlag(Parameters.ThrowIfNotFound);
            if (!TypeCache.TryGetConstructor(
                new TypeCache.ConstructrorKey(
                    type,
                    (Type[])bindings.Arguments,
                    bindings.ParameterModifiers),
                out ConstructorInfo? ctor))
            {
                ctor = type.GetConstructor(bindings, throwIfNotFound: false);

                if (ctor is null)
                {
                    if (throwIfNotFound)
                        throw new MemberNotFoundException(
                            type,
                            MemberType.Constructor,
                            bindings);

                    return null!;
                }

                if (parameters.HasFlag(Parameters.CacheConstructor))
                    TypeCache.TryCacheMember(ctor);
            }

            object?[] ctorArgs = (object?[])bindings.Arguments;
            return ctor.Invoke(ctorArgs);
        }
        public static object Create(Type type,
                                    ExplicitArguments args,
                                    Parameters parameters = Parameters.Default)
        {
            return Create(type, new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = args
            }, parameters);
        }

        public static T Create<T>(ConstructorBindings constructorParams,
            Parameters parameters = Parameters.Default)
        {
            return (T)Create(typeof(T), constructorParams, parameters);
        }
        public static T Create<T>(Type type,
                                  ExplicitArguments args,
                                  Parameters parameters = Parameters.Default)
        {
            return (T)Create(type, args, parameters);
        }
    }
}
