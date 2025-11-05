using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public static class InputActionRxFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static IInputActionRx<T> Create<T>(InputAction inputAction)
            where T : struct
        {
            Guard.IsNotNull(inputAction, nameof(inputAction));

            if (inputAction.type == InputActionType.Button || inputAction.expectedControlType == "Button")
                return new ButtonActionRx(inputAction).As<IInputActionRx<T>>();

            return new InputActionRx<T>(inputAction);
        }
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IInputActionRx Create(Type valueType, InputAction inputAction)
        {
            if (inputAction is null)
                throw new ArgumentNullException(nameof(inputAction));
            if (valueType is null)
                return new InputActionRx(inputAction);

            return typeof(InputActionRxFactory).Reflect()
                                               .Name(nameof(Create))
                                               .Arguments(inputAction)
                                               .GenericArguments(valueType)
                                               .Method()
                                               .Strict()
                                               .Invoke(null, Range.From(inputAction))
                                               .As<IInputActionRx>();
        }
    }
}
