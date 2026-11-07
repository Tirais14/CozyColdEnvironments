using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceFactoryBinderNonGeneric 
        :
        IServiceBinder,
        IServiceBindingNotBoundStateAccessor,
        IServiceBindingFactoryProvider,
        IServiceBindingLazyStateAccessor,
        ITransientBindable,
        ISingleBindable
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
