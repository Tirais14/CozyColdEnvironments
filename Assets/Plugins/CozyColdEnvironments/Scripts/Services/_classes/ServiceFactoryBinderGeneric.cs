using CCEnvs.Patterns.Factories;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryBinderGeneric<T>
        :
        IServiceBinder<T>, 
        IServiceBindingNotBoundStateProvider, 
        IServiceBindingFactoryProvider<T>, 
        IServiceBindingLazyStateProvider
    {
        public IFactory<T> Factory { get; }

        public bool IgnoreIfBound { get; set; }
        public bool IsLazy { get; set; } = true;

        public ServiceFactoryBinderGeneric(IFactory<T> factory)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            Factory = factory;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this, BindingType.Single);
        }

        public IDisposable AsTransient()
        {
            return CCServices.Bind(this, BindingType.Transient);
        }

        public ServiceFactoryBinderGeneric<T> IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public ServiceFactoryBinderGeneric<T> Lazy()
        {
            IsLazy = true;
            return this;
        }

        public ServiceFactoryBinderGeneric<T> NonLazy()
        {
            IsLazy = false;
            return this;
        }

        public ServiceFactoryIDBinderGeneric<T> WithID(object id)
        {
            return new ServiceFactoryIDBinderGeneric<T>(Factory, id)
            {
                IgnoreIfBound = IgnoreIfBound,
                IsLazy = IsLazy,
            };
        }
    }
}
