using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceInstanceIDBinderGeneric<T> : IServiceBinder, IServiceBinderIDProvider, IServiceBinderIfNotBoundProvider
    {
        public Type Contract => typeof(T);

        public T Instance { get; }

        public object ID { get; }

        public bool IgnoreIfBound { get; set; }

        object IServiceBinder.Instance => Instance!;

        public ServiceInstanceIDBinderGeneric(T instance, object id)
        {
            CC.Guard.IsNotNull(instance, nameof(instance));
            CC.Guard.IsNotNull(id, nameof(id));

            Instance = instance;
            ID = id;
        }

        public ServiceInstanceIDBinderGeneric<T> IfNotBound()
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
