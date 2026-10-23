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
        private static readonly MemoryCache emptyConstructorCache = new(
            new MemoryCacheOptions()
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

            if (emptyConstructorCache.TryGetValue<ConstructorInfo>(contract, out var cachedEmptyCtor) &&
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
            using var cacheEntry = emptyConstructorCache.CreateEntry(contract);
            cacheEntry.Value = emptyCtor;
            cacheEntry.AbsoluteExpirationRelativeToNow = 6.Minutes();
            return instance;
        }
    }
}
