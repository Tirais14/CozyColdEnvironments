#nullable enable
using System;

namespace CCEnvs.Services
{
    public class ServiceInstanceBinderGeneric<T> 
        :
        IServiceBinder<T>,
        IServiceBindingNotBoundStateProvider,
        IServiceBindingInstanceProvider<T>
    {
        public T Instance { get; }

        public bool IgnoreIfBound { get; set; }

        public ServiceInstanceBinderGeneric(T instance)
        {
            CC.Guard.IsNotNull(instance, nameof(instance));

            Instance = instance;
        }

        public ServiceInstanceBinderGeneric<T> IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this, BindingType.Single);
        }

        public ServiceInstanceIDBinderGeneric<T> WithID(object id)
        {
            return new ServiceInstanceIDBinderGeneric<T>(Instance, id);
        }
    }
}
