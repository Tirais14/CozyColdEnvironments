#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Services
{
    public class ServiceInterfacesInstanceIDBinderNonGeneric : IServiceBinder, IServiceBinderIDProvider, IServiceBinderIfNotBoundProvider
    {
        public Type Contract { get; }

        public object Instance { get; }

        public IReadOnlyList<Type> Interfaces { get; }

        public object ID { get; }

        public bool IgnoreIfBound { get; set; }

        public ServiceInterfacesInstanceIDBinderNonGeneric(
            Type contract,
            IEnumerable<Type> interfaces,
            object instance,
            object id
            )
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(interfaces, nameof(interfaces));
            CC.Guard.IsNotNull(instance, nameof(instance));
            CC.Guard.IsNotNull(id, nameof(id));

            Contract = contract;
            Interfaces = interfaces.ToArray();
            Instance = instance;
            ID = id;
        }

        public ServiceInterfacesInstanceIDBinderNonGeneric IfNotBound()
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
