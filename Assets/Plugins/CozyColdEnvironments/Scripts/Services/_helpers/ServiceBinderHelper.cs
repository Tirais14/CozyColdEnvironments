#nullable enable
using CCEnvs.Diagnostics;
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

            var cacheID = ("EmptyConstructor", contract);
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
            var ctor = GetResolveConstructor(contract);
            var arguments = ResolveServices(ctor);
            return ctor.Invoke(arguments.ToArray());
        }

        public static MethodInfo GetResolveMethod(
            Type type,
            string name,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
            StringMatchSettings nameMatchSettings = StringMatchSettings.Ordinal
            )
        {
            Guard.IsNotNull(type);
            Guard.IsNotNullOrWhiteSpace(name, nameof(name));

            var cacheID = ("Method", type, name, bindingFlags, nameMatchSettings);
            if (cache.TryGetValue<MethodInfo>(cacheID, out var cachedMethod) &&
                cachedMethod is not null)
                return cachedMethod;

            foreach (var method in type.GetMethods(bindingFlags).OrderByDescending(x => x.IsDefined<ResolveAttribute>()))
            {
                if (!method.Name.Match(name, nameMatchSettings))
                    continue;

                using var cacheEntry = cache.CreateEntry(cacheID);
                cacheEntry.Value = method;
                cacheEntry.SlidingExpiration = 6.Minutes();
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

        public static ConstructorInfo GetResolveConstructor(
            Type type,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public
            )
        {
            Guard.IsNotNull(type, nameof(type));

            var cacheID = ("Constructor", type, bindingFlags);
            if (cache.TryGetValue<ConstructorInfo>(cacheID, out var cachedCtor) &&
                cachedCtor.IsNotNull())
                return cachedCtor;

            foreach (var ctor in type.GetConstructors(bindingFlags).OrderByDescending(x => x.IsDefined<ResolveAttribute>()))
            {
                using var cacheEntry = cache.CreateEntry(cacheID);
                cacheEntry.Value = ctor;
                cacheEntry.SlidingExpiration = 6.Minutes();
                return ctor;
            }

            throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Not found constructor")
                .AddProperty("Type", type)
                .AddProperty("BindingFlags", bindingFlags)
                .ToStringAndDispose()
                );
        }

        public static IList<object?> ResolveServices(MethodBase method)
        {
            Guard.IsNotNull(method, nameof(method));

            var parameters = method.GetParameters();

            if (parameters.Length == 0)
                return Array.Empty<object>();

            var results = new List<object?>(parameters.Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo param = parameters[i];

                if (param.IsDefined(typeof(ResolveOptionalAttribute), inherit: true))
                {
                    if (param.GetCustomAttribute<ResolveIDAttribute>(inherit: true).IsNotNull(out var resolveID))
                        results.Add(CCServices.TryResolve(param.ParameterType, resolveID.ID));
                    else
                        results.Add(CCServices.TryResolve(param.ParameterType));
                }
                else
                {
                    if (param.GetCustomAttribute<ResolveIDAttribute>(inherit: true).IsNotNull(out var resolveID))
                        results.Add(CCServices.Resolve(param.ParameterType, resolveID.ID));
                    else
                        results.Add(CCServices.Resolve(param.ParameterType));
                }
            }

            return results;
        }
    }
}
