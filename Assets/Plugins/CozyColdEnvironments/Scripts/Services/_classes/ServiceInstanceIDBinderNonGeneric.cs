using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceInstanceIDBinderNonGeneric
        :
        IServiceBinder,
        IServiceBindingIDProvider,
        IServiceBindingNotBoundStateProvider,
        IServiceBindingInstanceProvider
    {
        public Type Contract { get; }

        public object Instance { get; }

        public object ID { get; }

        public bool IgnoreIfBound { get; private set; }

        public ServiceInstanceIDBinderNonGeneric(Type contract, object instance, object id)
        {
            Guard.IsNotNull(contract);
            CC.Guard.IsNotNull(instance, nameof(instance));
            CC.Guard.IsNotNull(id, nameof(id));

            Contract = contract;
            Instance = instance;
            ID = id;
        }

        public ServiceInstanceIDBinderNonGeneric IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this, BindingType.Single);
        }
    }
}
