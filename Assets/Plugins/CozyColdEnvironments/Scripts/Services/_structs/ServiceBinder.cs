using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceBinder : ServiceBinderBase<ServiceBinder>
    {
        public ServiceBinder(Type contract) : base(contract) { }
    }

    public class ServiceBinder<TContract> : ServiceBinderBase<ServiceBinder<TContract>>
    {
        public ServiceBinder() : base(typeof(TContract)) { }

        public ServiceBinder<TContract> FromInstance(TContract instance)
        {
            Instance = instance;
            return this;
        }
    }
}
