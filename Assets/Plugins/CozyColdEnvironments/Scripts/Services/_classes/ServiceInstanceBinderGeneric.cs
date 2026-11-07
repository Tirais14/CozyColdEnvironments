#nullable enable
using System;

namespace CCEnvs.Services
{
    public class ServiceInstanceBinderGeneric<T> 
        :
        IServiceBinder<T>,
        IServiceBindingNotBoundStateAccessor,
        IServiceBindingInstanceProvider<T>,
        ISingleBindable
    {
        public T Instance { get; }

        public bool IgnoreIfBound { get; set; }

        public ServiceInstanceBinderGeneric(T instance)
        {
            CC.Guard.IsNotNull(instance, nameof(instance));

            Instance = instance;
        }

        public ServiceInstanceIDBinderGeneric<T> WithID(object id)
        {
            return new ServiceInstanceIDBinderGeneric<T>(Instance, id);
        }
    }
}
