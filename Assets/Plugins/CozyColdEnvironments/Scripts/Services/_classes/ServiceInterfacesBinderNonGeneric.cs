using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceInterfacesBinderNonGeneric
    {
        public Type Contract { get; }

        public IReadOnlyList<Type> Interfaces { get; }

        public ServiceInterfacesBinderNonGeneric(
            Type contract,
            IEnumerable<Type> interfaces
            )
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(interfaces, nameof(interfaces));

            Interfaces = interfaces.ToArray();
        }
    }
}
