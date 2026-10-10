using CCEnvs.Services;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    public class ComponentHierarchyServiceBinder : IntermediateServiceBinder<ComponentHierarchyServiceBinder>
    {
        public object Target { get; }

        public ComponentHierarchyServiceBinder(ServiceBinderBase binderBase, bool includeInactive) : base(binderBase)
        {
            Target = GameObjectQuery.Scene.IncludeInactive(includeInactive).Component(binderBase.BaseContract).Strict();
        }
    }
}
