#if VCONTAINER_PLUGIN
using CCEnvs.UnityX;
using CommunityToolkit.Diagnostics;
#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;
#endif
using System;
using System.Threading;
using UnityEngine;
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

        public static void NonLazy(this IContainerBuilder builder, Type type, object? key = null)
        {
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Resolve(type, key);
            });
        }
        public static void NonLazy(this IContainerBuilder builder, Type type, int delayFrameCount, CancellationToken cancellationToken = default, object? key = null)
        {
            if (delayFrameCount <= 0)
            {
                builder.NonLazy(type, key);
                return;
            }

            builder.RegisterBuildCallback(resolver =>
            {
#if UNITASK_PLUGIN
                UniTask.Create(async () =>
                {
                    await UniTask.DelayFrame(delayFrameCount, cancellationToken: cancellationToken); ;
                    resolver.Resolve(type, key);
                });
#else
                CoroutineHelper.Action((actions) =>
                {
                    actions.Add(new WaitForFrames(delayFrameCount));
                    actions.Add(new Action(() => resolver.Resolve(type, key)));
                });
#endif
            });
        }
        public static void NonLazy<T>(this IContainerBuilder builder, object? key = null)
        {
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Resolve<T>(key);
            });
        }
        public static void NonLazy<T>(this IContainerBuilder builder, int delayFrameCount, CancellationToken cancellationToken = default, object? key = null)
        {
            if (delayFrameCount <= 0)
            {
                builder.NonLazy<T>(key);
                return;
            }

            builder.RegisterBuildCallback(resolver =>
            {
#if UNITASK_PLUGIN
                UniTask.Create(async () =>
                {
                    await UniTask.DelayFrame(delayFrameCount, cancellationToken: cancellationToken);
                    resolver.Resolve<T>(key);
                });
#else
                CoroutineHelper.Action((actions) =>
                {
                    actions.Add(new WaitForFrames(delayFrameCount));
                    actions.Add(new Action(() => resolver.Resolve<T>(key)));
                });
#endif
            });
        }
    }
}
#endif
