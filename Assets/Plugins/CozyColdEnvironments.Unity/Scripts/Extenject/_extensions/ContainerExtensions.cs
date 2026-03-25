using CCEnvs.Reflection;
using CCEnvs.Unity.InputSystem.Rx;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

#nullable enable
namespace CCEnvs.Zenject
{
    public static class ContainerExtensions
    {
        public static void BindInputActionAsset(
            this DiContainer container,
            InputActionAsset inputActionAsset,
            bool bindWithName = false 
            )
        {
            Guard.IsNotNull(container, nameof(container));

            var assetBinding = container.BindInstance(inputActionAsset);

            if (bindWithName)
                assetBinding.WithId(inputActionAsset.name).AsCached();
            else
                assetBinding.AsSingle();

            foreach (var actionMap in inputActionAsset.actionMaps)
            {
                container.BindInstance(actionMap)
                    .WithId(actionMap.name)
                    .AsCached()
                    .NonLazy();
            }
        }

        public static void BindInputHandlers(
            this DiContainer container,
            InputActionAsset inputActionAsset,
            params (string ActionMapName, Type InputHandlerType)[] inputHandlerInfos
            )
        {
            Guard.IsNotNull(container, nameof(container));

            var handlers = InputHandlerRxFactory.CreateInputHandlers(
                inputActionAsset,
                inputHandlerInfos
                );

            foreach (var handler in handlers)
            {
                //container.BindInstance(handler.ActionMap)
                //    .WithId(handler.ActionMap.name)
                //    .AsCached()
                //    .NonLazy();

                container.Bind(handler.GetType())
                    .FromInstance(handler)
                    .AsSingle()
                    .NonLazy();
            }
        }

        private readonly static Lazy<Dictionary<DiContainer, WeakReference<LifetimeEventEmitter>>> lifetimeEventEmitters = new(() => new());

        public static void BindLifetimeInterfacesTo(
            this DiContainer container,
            Type contract
            )
        {
            Guard.IsNotNull(container, nameof(container));
            Guard.IsNotNull(contract, nameof(contract));

            if (contract.IsType<IInitializable>())
                container.Bind<IInitializable>().To(contract).FromResolve();

#if UNITASK_PLUGIN
            if (contract.IsType<IStartable>())
            {
                GetOrCreateLifetimeEventEmitter(container);
                container.Bind<IStartable>().To(contract).FromResolve();
            }
#endif

            if (contract.IsType<ITickable>())
                container.Bind<ITickable>().To(contract).FromResolve();

            if (contract.IsType<IFixedTickable>())
                container.Bind<IFixedTickable>().To(contract).FromResolve();

            if (contract.IsType<ILateTickable>())
                container.Bind<ILateTickable>().To(contract).FromResolve();
        }

        private static LifetimeEventEmitter GetOrCreateLifetimeEventEmitter(DiContainer container)
        {
            if (!lifetimeEventEmitters.Value.TryGetValue(container, out var emitterRef)
                ||
                !emitterRef.TryGetTarget(out var emitter))
            {
                var go = new GameObject(
                    "[CCEnvs.Zenject]LifetimeEventEmitter",
                    typeof(LifetimeEventEmitter)
                    );

                if (container.HasBinding<LifetimeEventEmitter>())
                    container.Unbind<LifetimeEventEmitter>();

                emitter = go.GetComponent<LifetimeEventEmitter>();
                emitter.diContainer = container;

                emitterRef = new WeakReference<LifetimeEventEmitter>(emitter);

                lifetimeEventEmitters.Value[container] = emitterRef;

                container.BindInterfacesAndSelfTo<LifetimeEventEmitter>()
                    .FromInstance(emitter)
                    .AsCached();
            }

            return emitter;
        }

        public static void BindLifetimeInterfacesTo<TContract>(this DiContainer container)
        {
            BindLifetimeInterfacesTo(container, typeof(TContract));
        }

        public static ConcreteIdBinderNonGeneric BindLifetimeInterfacesAndSelfTo(
            this DiContainer container,
            Type contract
            )
        {
            container.BindLifetimeInterfacesTo(contract);

            return container.Bind(contract);
        }

        public static ConcreteIdBinderGeneric<TContract> BindLifetimeInterfacesAndSelfTo<TContract>(this DiContainer container)
        {
            container.BindLifetimeInterfacesTo(typeof(TContract));

            return container.Bind<TContract>();
        }
    }
}
