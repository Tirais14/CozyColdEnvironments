using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceBinderBase
    {
        private readonly Type baseContract;

        public Type BaseContract => baseContract;

        public ServiceBinderBase(Type contract)
        {
            Guard.IsNotNull(contract, nameof(contract));
            baseContract = contract;
        }

        public InstanceServiceBinder FromInstance(object instance)
        {
            return new InstanceServiceBinder(this, instance);
        }

        public FactoryServiceBinder AsFactory(IFactory<object> factory)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            return new FactoryServiceBinder(this, factory);
        }
    }

    public class ServiceBinderBase<TContract> : ServiceBinderBase
    {
        public ServiceBinderBase() : base(typeof(TContract))
        {
        }

        public InstanceServiceBinder<TContract> FromInstance(TContract instance)
        {
            return new InstanceServiceBinder<TContract>(this, instance);
        }

        public FactoryServiceBinder<TContract> AsFactory(IFactory<TContract> factory)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            return new FactoryServiceBinder<TContract>(this, factory);
        }
    }
}
