#if VCONTAINER_PLUGIN
using CCEnvs.UnityX;
using CommunityToolkit.Diagnostics;
using System;
using VContainer;
using VContainer.Unity;

#nullable enable
namespace CCEnvs.Unity.VContainer
{
    public static class IContainerBuilderExtensions
    {
        public static RegistrationBuilder RegisterComponentInHierarchyCustom(this IContainerBuilder builder, Type componentType, bool includeInactive = false)
        {
            CC.Guard.IsNotNull(builder, nameof(builder));
            Guard.IsNotNull(componentType, nameof(componentType));
            var component = GameObjectQuery.Scene.IncludeInactive(includeInactive).Component(componentType).Strict();
            builder.RegisterBuildCallback(container =>
            {
                container.Inject(component);
            });

            return builder.RegisterInstance(component, componentType);
        }

        public static RegistrationBuilder RegisterComponentInHierarchyCustom<T>(this IContainerBuilder builder, bool includeInactive = false)
        {
            CC.Guard.IsNotNull(builder, nameof(builder));
            
            var component = GameObjectQuery.Scene.IncludeInactive(includeInactive).Component<T>().Strict();
            builder.RegisterBuildCallback(container =>
            {
                container.Inject(component);
            });

            return builder.RegisterInstance(component);
        }
    }
}
#endif
