using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryIDBinderNonGeneric
        :
        IServiceBinder,
        IServiceBindingNotBoundStateProvider, 
        IServiceBindingIDProvider, 
        IServiceBindingFactoryProvider,
        IServiceBindingLazyStateProvider
    {
        public Type Contract { get; }

        public bool IgnoreIfBound { get; set; }
        public bool IsLazy { get; set; } = true;

        public object ID { get; }

        public IFactory Factory { get; }

        public ServiceFactoryIDBinderNonGeneric(Type contract, IFactory factory, object id)
        {
            Guard.IsNotNull(contract, nameof(contract));
            CC.Guard.IsNotNull(factory, nameof(factory));
            CC.Guard.IsNotNull(id, nameof(id));

            Contract = contract;
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

        public ServiceFactoryIDBinderNonGeneric IfNotBound()
        {
            IgnoreIfBound = true;
            return this;
        }

        public ServiceFactoryIDBinderNonGeneric Lazy()
        {
            IsLazy = true;
            return this;
        }

        public ServiceFactoryIDBinderNonGeneric NonLazy()
        {
            IsLazy = false;
            return this; ;
        }
    }
}
