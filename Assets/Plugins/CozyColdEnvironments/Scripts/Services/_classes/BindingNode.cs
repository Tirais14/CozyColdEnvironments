using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Services
{
    public class BindingNode
    {
        public object Target { get; }

        public BindingType BindingType { get; }

        public BindingNode(
            object target,
            BindingType bindingType
            )
        {
            Guard.IsNotNull(target);

            Target = target;
            BindingType = bindingType;
        }

        public object GetInstance() => Target;
    }
}
