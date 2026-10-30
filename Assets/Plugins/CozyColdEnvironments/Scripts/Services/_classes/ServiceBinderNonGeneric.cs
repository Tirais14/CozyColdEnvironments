using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services   
{
    public class ServiceBinderNonGeneric
    {
        public Type Contract { get; }
        public Type? ToContract { get; set; }

        public ServiceBinderNonGeneric(Type contract)
        {
            Guard.IsNotNull(contract);

            Contract = contract;
        }

        public ServiceBinderNonGeneric To(Type? contract)
        {
            ToContract = contract;
            return this;
        }

        public ServiceInstanceBinderNonGeneric FromInstance(object instance)
        {
            return new ServiceInstanceBinderNonGeneric(ToContract ?? Contract, instance);
        }

        public ServiceFactoryBinderNonGeneric FromFactory(IFactory factory)
        {
            return new ServiceFactoryBinderNonGeneric(ToContract ?? Contract, factory);
        }

        public ServiceFactoryBinderNonGeneric FromMethod(Func<object> factory)
        {
            return new ServiceFactoryBinderNonGeneric(ToContract ?? Contract, Factory.Create(factory));
        }

        public ServiceFactoryBinderNonGeneric FromNew()
        {
            return new ServiceFactoryBinderNonGeneric(
                ToContract ?? Contract,
                Factory.Create(() => ServiceBinderHelper.CreateByConstructor(Contract))
                );
        }
    }
}
