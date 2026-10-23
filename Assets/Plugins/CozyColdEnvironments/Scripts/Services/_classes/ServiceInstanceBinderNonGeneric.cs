using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceInstanceBinderNonGeneric
        :
        IServiceBinder,
        IServiceBindingNotBoundStateProvider,
        IServiceBindingInstanceProvider
    {
        public Type Contract { get; }

        public object Instance { get; }

        public bool IgnoreIfBound { get; private set; }

        public ServiceInstanceBinderNonGeneric(Type contract, object instance)
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(instance, nameof(instance));

            Contract = contract;
            Instance = instance;
        }

        public ServiceInstanceBinderNonGeneric IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this, BindingType.Single);
        }

        public ServiceInstanceIDBinderNonGeneric WithID(object id)
        {
            return new ServiceInstanceIDBinderNonGeneric(Contract, Instance, id);
        }
    }
}
