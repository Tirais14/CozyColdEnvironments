#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCEnvs.Services
{
    public static class ServiceBinderHelper
    {
        private const string CONSTRUCTOR_CACHE_ID = "Constructor";
        private const string EMPTY_CONSTRUCTOR_CACHE_ID = "Empty Constructor";
        private const string METHOD_CACHE_ID = "Method";
        private const string METHOD_PARAMETERS_CACHE_ID = "Method Parameters";

        private static readonly MemoryCache cache = new(
            new MemoryCacheOptions
            {
            ExpirationScanFrequency = 2.Minutes()
            });

        public static IEnumerable<Type> FindInterfaces(
            Type contract,
            string? interfaceName = null,
            StringMatchSettings nameMatchSettings = StringMatchSettings.PartialFromStart,
            string? excludeInterfaceName = null,
            StringMatchSettings excludeNameMatchSettings = StringMatchSettings.PartialFromStart,
            string? excludeNamespace = null,
            StringMatchSettings excludeNamespaceMatchSettings = StringMatchSettings.PartialFromStart
            )
        {
            Guard.IsNotNull(contract, nameof(contract));

            var ifaces = contract.GetInterfaces();

            if (interfaceName.IsNullOrEmpty() || nameMatchSettings == StringMatchSettings.None)
                return ifaces;

            IEnumerable<Type> results = ifaces;

            if (interfaceName.IsNotNullOrWhiteSpace() || nameMatchSettings == StringMatchSettings.None)
                results = results.Where(iface => iface.Name.Match(interfaceName, nameMatchSettings));

            if (excludeInterfaceName.IsNotNullOrWhiteSpace() || excludeNameMatchSettings == StringMatchSettings.None)
                results = results.Where(iface => !iface.Name.Match(excludeInterfaceName, excludeNameMatchSettings));

            if (excludeNamespace.IsNotNullOrWhiteSpace() || excludeNamespaceMatchSettings == StringMatchSettings.None)
                results = results.Where(iface => !iface.Namespace.Match(excludeNamespace, excludeNamespaceMatchSettings));

            return ifaces;

        }
        public static IEnumerable<Type> FindInterfaces<TInterface>(Type contract)
        {
            return FindInterfaces(contract, typeof(TInterface).GetName(TypeNameConvertingAttributes.None), StringMatchSettings.Ordinal);
        }

        public static object CreateByEmptyConstructor(Type contract)
        {
            if (contract.IsInterface || contract.IsAbstract)
                throw new ArgumentException("Contract is abstract type");

            object cacheID = (EMPTY_CONSTRUCTOR_CACHE_ID, contract);
            if (cache.TryGetValue<ConstructorInfo>(cacheID, out var cachedEmptyCtor) &&
                cachedEmptyCtor is not null
                )
                return cachedEmptyCtor.Invoke(Array.Empty<object>());

            ConstructorInfo emptyCtor =
                (from ctor in contract.GetConstructors()
                 let parameters = ctor.GetParameters()
                 where parameters.Length == 0 || parameters.All(x => x.HasDefaultValue)
                 select ctor
                )
                .FirstOrDefault() ??
                throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Type doesn't contains any empty constructor")
                    .AddProperty("Type", contract)
                    .ToStringAndDispose()
                    );

            var instance = emptyCtor.Invoke(Array.Empty<object>());
            using var cacheEntry = cache.CreateEntry(cacheID);
            cacheEntry.Value = emptyCtor;
            cacheEntry.AbsoluteExpirationRelativeToNow = 6.Minutes();
            return instance;
        }

        public static object CreateByConstructor(Type contract)
        {
            Guard.IsNotNull(contract, nameof(contract));
            var ctor = ResolveConstructor(contract);
            var arguments = ResolveServices(ctor);
            return ctor.Invoke(arguments.ToArray());
        }

        public static MethodInfo ResolveMethod(
            Type type,
            string name,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
            StringMatchSettings nameMatchSettings = StringMatchSettings.Ordinal,
            bool cached = false
            )
        {
            Guard.IsNotNull(type);
            Guard.IsNotNullOrWhiteSpace(name, nameof(name));

            object cacheID = (METHOD_CACHE_ID, type, name, bindingFlags, nameMatchSettings);
            if (cache.TryGetValue<MethodInfo>(cacheID, out var cachedMethod) &&
                cachedMethod is not null)
                return cachedMethod;

            foreach (var method in type.GetMethods(bindingFlags).OrderByDescending(x => x.IsDefined<ResolveAttribute>()))
            {
                if (!method.Name.Match(name, nameMatchSettings))
                    continue;

                if (cached)
                {
                    using var cacheEntry = cache.CreateEntry(cacheID);
                    cacheEntry.Value = method;
                    cacheEntry.SlidingExpiration = 6.Minutes();
                }

                return method;
            }

            throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Not found any resolve method")
                .AddProperty("Type", type)
                .AddProperty("Name", name)
                .AddProperty("NameMatchSettings", nameMatchSettings)
                .ToStringAndDispose()
                );
        }

        public static ConstructorInfo ResolveConstructor(
            Type type,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public
            )
        {
            Guard.IsNotNull(type, nameof(type));

            object cacheID = (CONSTRUCTOR_CACHE_ID, type, bindingFlags);

            if (cache.TryGetValue<ConstructorInfo>(cacheID, out var cachedCtor) &&
                cachedCtor.IsNotNull())
                return cachedCtor;

            var ctors = type.GetConstructors(bindingFlags);

            void cacheConstructor(ConstructorInfo ctor)
            {
                using var cacheEntry = cache.CreateEntry(cacheID);
                cacheEntry.Value = ctor;
                cacheEntry.SlidingExpiration = 6.Minutes();
            }

            ConstructorInfo? ctor;

            if (ctors.Length == 1)
            {
                ctor = ctors[0];
                cacheConstructor(ctor);
                return ctor;
            }
            else if (type.IsValueType && ctors.Length == 2)
            {
                ctor = ctors.First(x => x.GetParameters().Length != 0);
                cacheConstructor(ctor);
                return ctor;
            }
            else if (ctors.FirstOrDefault(x => x.IsDefined<ResolveAttribute>(inherit: true)).IsNotNull(out ctor))
            {
                cacheConstructor(ctor);
                return ctor;
            }
            else
            {
                foreach (var ctorInfo in ctors.Select(ctor => (ctor, parameters: ctor.GetParameters())).OrderByDescending(x => x.parameters.Length))
                {
                    if (!CanResolveServices(ctorInfo.parameters))
                        continue;

                    return ctorInfo.ctor;
                }
            }

            throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Not found constructor")
                .AddProperty("Type", type)
                .AddProperty("BindingFlags", bindingFlags)
                .ToStringAndDispose()
                );
        }

        public static bool CanResolveServices(ParameterInfo[] parameters)
        {
            foreach (var param in parameters)
            {
                if (param.GetCustomAttribute<ResolveIDAttribute>(inherit: true).IsNotNull(out var resolveID))
                {
                    if (!CCServices.HasBinding(param.ParameterType, resolveID.ID) &&
                        !param.HasDefaultValue &&
                        !param.IsDefined(typeof(ResolveOptionalAttribute), inherit: true)
                        )
                        return false;
                }
                else if (!CCServices.HasBinding(param.ParameterType) && param.IsDefined(typeof(ResolveOptionalAttribute), inherit: true))
                    return false;

            }

            return true;
        }

        public static bool CanResolveServices(MethodBase method)
        {
            return CanResolveServices(method.GetParameters());
        }

        public static IList<object?> ResolveServices(MethodBase method, bool cached = false)
        {
            Guard.IsNotNull(method, nameof(method));

            ParameterInfo[]? parameters;

            object cacheID = (METHOD_PARAMETERS_CACHE_ID, method);

            if (!cache.TryGetValue(cacheID, out parameters) || parameters is null)
            {
                parameters = method.GetParameters();

                if (cached)
                {
                    using var cacheEntry = cache.CreateEntry(cacheID);
                    cacheEntry.Value = parameters;
                    cacheEntry.SlidingExpiration = 6.Minutes();
                }
            }

            if (parameters.Length == 0)
                return Array.Empty<object>();

            var results = new List<object?>(parameters.Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo param = parameters[i];

                if (param.ParameterType == method.DeclaringType)
                {
                    typeof(ServiceBinderHelper).PrintWarning(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Found circular dependecy")
                        .AddProperty("Contract", param.ParameterType)
                        .ToStringAndDispose()
                        );
                }

                if (param.HasDefaultValue || param.IsDefined(typeof(ResolveOptionalAttribute), inherit: true))
                {
                    var resolveID = param.GetCustomAttribute<ResolveIDAttribute>(inherit: true)?.ID;

                    if (CCServices.TryResolveOut(param.ParameterType, out var resolved, resolveID))
                        results.Add(resolved);
                    else
                        results.Add(param.HasDefaultValue ? param.DefaultValue : TypeHelper.GetDefaultValue(param.ParameterType));
                }
                else
                {
                    var resolveID = param.GetCustomAttribute<ResolveIDAttribute>(inherit: true)?.ID;

                    if (!CCServices.TryResolveOut(param.ParameterType, out var resolved, resolveID))
                    {
                        throw new BindingNotFoundException(DebugMessageBuilder.CreatePooled()
                            .AddMessage("Not found binding")
                            .AddProperty("Contract", param.ParameterType)
                            .AddPredicatedProperty(resolveID is not null, "ID", resolveID)
                            .AddProperty("Target", method.DeclaringType)
                            .ToStringAndDispose()
                            );
                    }

                    results.Add(resolved);
                }
            }

            return results;
        }
    }
}
