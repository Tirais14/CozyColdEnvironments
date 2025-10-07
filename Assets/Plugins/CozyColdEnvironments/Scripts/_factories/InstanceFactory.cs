using CCEnvs.Attributes;
using CCEnvs.Cacheables;
using CCEnvs.Collections;
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using static CCEnvs.BindingFlagsDefault;

#nullable enable
namespace CCEnvs
{
    public static class InstanceFactory
    {
        [Flags]
        public enum Parameters
        {
            None,
            CacheConstructor,
            ThrowIfNotFound = 2,
            NonPublic = 4,
            Default = ThrowIfNotFound,
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static object Create(Type type,
                                    ExplicitArguments arguments = default,
                                    Parameters parameters = Parameters.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException($"Type {type.GetName()} is interface and not allowed to create.");
            if (type.IsAbstract)
                throw new ArgumentException($"Type {type.GetName()} is abstract and not allowed to create.");
            if (arguments.IsDefault())
                arguments = ExplicitArguments.Empty;

            bool nonPublic = parameters.IsFlagSetted(Parameters.NonPublic);
            BindingFlags bindingFlags = ResolveBindingFlags(nonPublic);
            if (!GetConstructorByCache(out MethodBase? ctor))
            {
                ctor = GetConstructor();

                if (ctor is null)
                {
                    if (parameters.HasFlag(Parameters.ThrowIfNotFound))
                        ThrowNotFound();

                    return null!;
                }

                if (parameters.HasFlag(Parameters.CacheConstructor))
                    TypeCache.TryCacheMember(ctor);
            }

            object?[] ctorArgs = (object?[])arguments;
            if (ctor is ConstructorInfo typedCtor)
                return typedCtor.Invoke(ctorArgs);

            return ctor.Invoke(null, ctorArgs);

            bool GetConstructorByCache([NotNullWhen(true)] out MethodBase? result)
            {
                TypeCache.Constructors.TryGetValue(
                    new MethodKey(
                        type,
                        (CCParameters)arguments,
                        arguments.GetParameterModifiers()),
                    out ConstructorInfo? ctor);

                result = ctor;

                if (result is null)
                {
                    TypeCache.Methods.TryGetValue(
                        new MethodKey(
                            type,
                            (CCParameters)arguments,
                            arguments.GetParameterModifiers()),
                        out MethodInfo? method);

                    result = method;
                }

                return result is not null;
            }

            void ThrowNotFound()
            {
                throw new ConstructorNotFoundException(
                    type,
                    bindingFlags,
                    (CCParameters)arguments);
            }

            MethodBase? GetConstructor()
            {
                bindingFlags |= BindingFlags.Instance;
                MethodBase? ctor = type.GetConstructor(bindingFlags,
                    binder: null,
                    arguments.GetTypes(),
                    Range.From(arguments.GetParameterModifiers()));

                if (ctor is null)
                {
                    bindingFlags = bindingFlags.ResetFlag(BindingFlags.Instance)
                                               .SetFlag(BindingFlags.Static);

                    MethodInfo[] methods =
                        (from x in type.ForceGetMethods(bindingFlags)
                         where x.IsDefined<InstanceConstructorAttribute>(inherit: true)
                         ||
                         x.Name == "Create"
                         where x.ReturnType.IsType(type)
                         select x)
                         .ToArray();

                    ctor = methods.FirstOrDefault(
                        x => x.GetCCParameters() == (CCParameters)arguments);
                }

                return ctor;
            }
        }
        public static T Create<T>(ExplicitArguments arguments = default,
                                  Parameters parameters = Parameters.Default,
                                  Type? type = null)
        {
            type ??= typeof(T);

            return (T)Create(type, arguments, parameters);
        }

        private static BindingFlags ResolveBindingFlags(bool nonPublic)
        {
            if (nonPublic)
                return BindingFlags.Public | BindingFlags.NonPublic;

            return BindingFlags.Public;
        }
    }
}
