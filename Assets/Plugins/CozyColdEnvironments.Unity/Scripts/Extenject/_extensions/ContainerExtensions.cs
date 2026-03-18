using CCEnvs.Unity.InputSystem.Rx;
using System;
using Zenject;

#nullable enable
namespace CCEnvs.Zenject
{
    public static class ContainerExtensions
    {
        public static void BindInputHandlers(
            this DiContainer container,
            string resourcesInputActionAssetPath,
            (string ActionMapName, Type InputHandlerType)[] inputHandlerInfos
            )
        {
            var handlers = InputHandlerRxFactory.CreateInputHandlers(
                resourcesInputActionAssetPath,
                inputHandlerInfos
                );

            var inputActionAsset = handlers[0].ActionMap.asset;

            container.BindInstance(inputActionAsset)
                .AsCached()
                .NonLazy()
                .IfNotBound();

            container.BindInstance(inputActionAsset)
                .WithId(inputActionAsset.name)
                .AsCached()
                .NonLazy()
                .IfNotBound();

            foreach (var handler in handlers)
            {
                container.BindInstance(handler.ActionMap)
                    .WithId(handler.ActionMap.name)
                    .AsCached()
                    .NonLazy()
                    .IfNotBound();

                container.Bind(handler.GetType())
                    .FromInstance(handler)
                    .AsSingle()
                    .NonLazy();
            }
        }
    }
}
