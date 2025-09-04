using System;
using UnityEngine.InputSystem;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Reactive
{
    public static class InputActionReactiveFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static IInputActionReactive<T> Create<T>(InputAction inputAction)
            where T : struct
        {
            if (inputAction is null)
                throw new ArgumentNullException(nameof(inputAction));

            return new InputActionReactive<T>(inputAction);
        }
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IInputActionReactive Create(Type? valueType, InputAction inputAction)
        {
            if (inputAction is null)
                throw new ArgumentNullException(nameof(inputAction));
            if (valueType is null)
                return new InputActionReactive(inputAction);

            return MethodInvoker.Invoke<IInputActionReactive>(
                new TypeValuePair(typeof(InputActionReactiveFactory)),
                nameof(Create),
                new ExplicitArguments(new ExplicitArgument(inputAction)),
                valueType)!;
        }
    }
}
