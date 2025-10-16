using System;
using UnityEngine.InputSystem;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public static class InputActionRxFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static IInputActionRx<T> Create<T>(InputAction inputAction)
            where T : struct
        {
            if (inputAction is null)
                throw new ArgumentNullException(nameof(inputAction));

            return new InputActionRx<T>(inputAction);
        }
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IInputActionRx Create(Type? valueType, InputAction inputAction)
        {
            if (inputAction is null)
                throw new ArgumentNullException(nameof(inputAction));
            if (valueType is null)
                return new InputActionRx(inputAction);

            return MethodInvoker.Invoke<IInputActionRx>(
                new TypeValuePair(typeof(InputActionRxFactory)),
                nameof(Create),
                new ExplicitArguments(new ExplicitArgument(inputAction)),
                valueType)!;
        }
    }
}
