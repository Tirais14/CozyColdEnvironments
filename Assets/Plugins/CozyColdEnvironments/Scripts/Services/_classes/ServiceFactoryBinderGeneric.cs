using CCEnvs.Patterns.Factories;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryBinderGeneric<T>
        :
        IServiceBinder<T>, 
        IServiceBindingNotBoundStateAccessor, 
        IServiceBindingFactoryProvider<T>, 
        IServiceBindingLazyStateAccessor,
        ITransientBindable,
        ISingleBindable
    {
        public IFactory<T> Factory { get; }

        public bool IgnoreIfBound { get; set; }
        public bool IsLazy { get; set; } = true;

        public ServiceFactoryBinderGeneric(IFactory<T> factory)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            Factory = factory;
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
