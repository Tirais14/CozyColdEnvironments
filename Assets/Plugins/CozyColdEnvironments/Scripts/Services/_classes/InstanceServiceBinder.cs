using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Services
{
    public class InstanceServiceBinder : IntermediateServiceBinder<InstanceServiceBinder>, IServiceBindingInfoProvider
    {
        public object Target { get; private set; }

        public bool IsFactory => false;

        public InstanceServiceBinder(ServiceBinderBase binderBase, object instance) 
            :
            base(binderBase)
        {
            Guard.IsNotNull(instance, nameof(instance));
            Target = instance;
        }
    }

    public class InstanceServiceBinder<TContract> : InstanceServiceBinder
    {
        public InstanceServiceBinder(ServiceBinderBase binderBase, TContract instance) 
            :
            base(binderBase, instance!)
        {
        }
    }
}
