using CCEnvs.Unity.InputSystem.Rx;
using CommunityToolkit.Diagnostics;
using System;
using UnityEngine.InputSystem;
using VContainer;

#nullable enable
namespace CCEnvs.Unity.VContainer
{
    public static class IContainerBuilderExtensions
    {
        public static void RegisterInputHandlers(
            this IContainerBuilder source,
            InputActionAsset inputActionAsset,
            params (string ActionMapName, Type InputHandlerType)[] inputHandlerInfos
            )
        {
            Guard.IsNotNull(source, nameof(source));

            var handlers = InputHandlerRxFactory.CreateInputHandlers(
                inputActionAsset,
                inputHandlerInfos
                );

            IInputHandlerRx handler;

            for (int i = 0; i < handlers.Count; i++)
            {
                handler = handlers[i];

                source.RegisterInstance(handler, handler.GetType());
            }
        }

        public static RegistrationBuilder RegisterInstanceAndQueueToInject(
            this IContainerBuilder source, 
            object instance,
            Type implementationType
            )
        {
            CC.Guard.IsNotNullSource(source);

            var regBuilder = source.RegisterInstance(instance, implementationType);

            source.RegisterBuildCallback((builder) =>
            {
                builder.Inject(instance);
            });

            return regBuilder;
        }

        public static RegistrationBuilder RegisterInstanceAndQueueToInject<TInstance>(
            this IContainerBuilder source,
            TInstance instance
            )
        {
            CC.Guard.IsNotNullSource(source);

            var regBuilder = source.RegisterInstance(instance);

            source.RegisterBuildCallback((builder) =>
            {
                builder.Inject(instance);
            });

            return regBuilder;
        }
    }
}
