#nullable enable
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Services
{
    public static class ServiceBinderHelper
    {
        public static IEnumerable<Type> FindInterfaces(
            Type contract,
            string? interfaceName,
            StringMatchSettings nameMatchSettings
            )
        {
            var ifaces = contract.GetInterfaces();

            if (interfaceName.IsNullOrEmpty() || nameMatchSettings == StringMatchSettings.None)
                return ifaces;

            return ifaces.Where(iface => iface.Name.Match(interfaceName, nameMatchSettings));

        }
        public static IEnumerable<Type> FindInterfaces<TInterface>(Type contract)
        {
            return FindInterfaces(contract, typeof(TInterface).GetName(TypeNameConvertingAttributes.None), StringMatchSettings.Ordinal);
        }
    }
}
