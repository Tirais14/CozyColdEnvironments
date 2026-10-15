using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services   
{
    public class ServiceBinderNonGeneric
    {
        public Type Contract { get; }

        public ServiceBinderNonGeneric(Type contract)
        {
            Guard.IsNotNull(contract);

            Contract = contract;
        }

        public ServiceInstanceBinderNonGeneric FromInstance(object instance)
        {
            return new ServiceInstanceBinderNonGeneric(Contract, instance);
        }
    }
}
