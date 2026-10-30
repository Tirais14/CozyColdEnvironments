#nullable enable
namespace CCEnvs.Services
{
    public class InstanceBindingNode : IBindingNode
    {
        private readonly object instance;

        public InstanceBindingNode(object instance)
        {
            CC.Guard.IsNotNull(instance, nameof(instance));

            this.instance = instance;
        }

        public object GetInstance() => instance;
    }
}
