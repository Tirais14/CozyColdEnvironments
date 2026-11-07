using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceInstanceBinderNonGeneric
        :
        IServiceBinder,
        IServiceBindingNotBoundStateAccessor,
        IServiceBindingInstanceProvider,
        ISingleBindable
    {
        public Type Contract { get; }

        public object Instance { get; }

        public bool IgnoreIfBound { get; set; }

        public ServiceInstanceBinderNonGeneric(Type contract, object instance)
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(instance, nameof(instance));

            Contract = contract;
            Instance = instance;
        }

        public ServiceInstanceIDBinderNonGeneric WithID(object id)
        {
            return new ServiceInstanceIDBinderNonGeneric(Contract, Instance, id);
        }
    }
}
