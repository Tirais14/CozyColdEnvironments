#nullable enable
using System;

namespace CCEnvs.Services
{
    public class ServiceInstanceBinderGeneric<T> : IServiceBinder, IServiceBinderIfNotBoundProvider
    {
        public Type Contract => typeof(T);

        public T Instance { get; }

        public bool IgnoreIfBound { get; set; }

        object IServiceBinder.Instance => Instance!;

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
            return CCServices.Bind(this);
        }

        public ServiceInstanceIDBinderGeneric<T> WithID(object id)
        {
            return new ServiceInstanceIDBinderGeneric<T>(Instance, id);
        }
    }
}
