using CCEnvs.Patterns.Factories;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryIDBinderGeneric<T> 
        :
        IServiceBinder<T>,
        IServiceBindingIDProvider,
        IServiceBindingNotBoundStateProvider,
        IServiceBindingFactoryProvider<T>,
        IServiceBindingLazyStateProvider
    {
        public IFactory<T> Factory { get; }

        public object ID { get; }

        public bool IgnoreIfBound { get; set; }
        public bool IsLazy { get; set; } = true;

        public ServiceFactoryIDBinderGeneric(IFactory<T> factory, object id)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            CC.Guard.IsNotNull(id, nameof(id));

            Factory = factory;
            ID = id;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this, BindingType.Single);
        }

        public IDisposable AsTransient()
        {
            return CCServices.Bind(this, BindingType.Transient);
        }

        public ServiceFactoryIDBinderGeneric<T> IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public ServiceFactoryIDBinderGeneric<T> Lazy()
        {
            IsLazy = true;
            return this;
        }

        public ServiceFactoryIDBinderGeneric<T> NonLazy()
        {
            IsLazy = false;
            return this;
        }
    }
}
