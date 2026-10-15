using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceInterfacesInstanceBinderNonGeneric : IServiceBinder, IServiceBinderIfNotBoundProvider, IServiceBinderInterfacesProvider
    {
        public Type Contract { get; }

        public IReadOnlyList<Type> Interfaces { get; }

        public object Instance { get; }

        public bool IgnoreIfBound { get; set; }

        public ServiceInterfacesInstanceBinderNonGeneric(
            Type contract,
            IEnumerable<Type> interfaces,
            object instance
            )
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(interfaces, nameof(interfaces));
            CC.Guard.IsNotNull(instance, nameof(instance));

            Contract = contract;
            Interfaces = interfaces.ToArray();
            Instance = instance;
        }

        public ServiceInterfacesInstanceBinderNonGeneric IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this);
        }
    }
}
