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
        IServiceBindingNotBoundStateAccessor,
        IServiceBindingInstanceProvider,
        ISingleBindable
    {
        public Type Contract { get; }

        public object Instance { get; }

        public object ID { get; }

        public bool IgnoreIfBound { get; set; }

        public ServiceInstanceIDBinderNonGeneric(Type contract, object instance, object id)
        {
            Guard.IsNotNull(contract);
            CC.Guard.IsNotNull(instance, nameof(instance));
            CC.Guard.IsNotNull(id, nameof(id));

            Contract = contract;
            Instance = instance;
            ID = id;
        }
    }
}
