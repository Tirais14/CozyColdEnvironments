using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryBinderNonGeneric 
        :
        IServiceBinder, 
        IServiceBindingNotBoundStateProvider,
        IServiceBindingFactoryProvider,
        IServiceBindingLazyStateProvider
    {
        public Type Contract { get; }

        public IFactory Factory { get; }

        public bool IgnoreIfBound { get; set; }
        public bool IsLazy { get; set; } = true;

        public ServiceFactoryBinderNonGeneric(Type contract, IFactory factory)
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(factory, nameof(factory));

            Contract = contract;
            Factory = factory;
        }

        public ServiceFactoryBinderNonGeneric IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public ServiceFactoryBinderNonGeneric Lazy()
        {
            IsLazy = true;
            return this;
        }

        public ServiceFactoryBinderNonGeneric NonLazy()
        {
            IsLazy = false;
            return this;
        }

        public IDisposable AsSingle()
        {
            return CCServices.Bind(this, BindingType.Single);
        }

        public IDisposable AsTransient()
        {
            return CCServices.Bind(this, BindingType.Transient);
        }

        public ServiceFactoryIDBinderNonGeneric WithID(object id)
        {
            return new ServiceFactoryIDBinderNonGeneric(Contract, Factory, id)
            {
                IgnoreIfBound = IgnoreIfBound,
                IsLazy = IsLazy,
            };
        }
    }
}
