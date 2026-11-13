using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryIDBinderGeneric<T> 
        :
        IServiceBinder<T>,
        IServiceBindingIDProvider,
        IServiceBindingNotBoundStateAccessor,
        IServiceBindingFactoryProvider<T>,
        IServiceBindingLazyStateAccessor,
        ISingleBindable,
        ITransientBindable,
        IServiceBindingOnFirstResolvedActionAccessor<T>
    {
        public OnResolvedAction<T>? EvOnFirstResolved { get; set; }

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
    }
}
