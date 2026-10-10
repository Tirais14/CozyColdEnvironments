using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Services
{
    public class FactoryServiceBinder : IntermediateServiceBinder<FactoryServiceBinder>
    {
        public object Target { get; }

        public bool IsFactory => true;

        public FactoryServiceBinder(ServiceBinderBase binderBase, IFactory<object> factory) : base(binderBase)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            Target = factory;
        }
    }

    public class FactoryServiceBinder<TContract> : FactoryServiceBinder
    {
        public FactoryServiceBinder(ServiceBinderBase binderBase, IFactory<TContract> factory)
            :
            base(binderBase, Patterns.Factories.Factory.Create(factory, factory => (object)factory.Create()!))
        {
        }
    }
}
