using System;
using UnityEngine.InputSystem;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
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
            {
                return MethodInvoker.Invoke<IInputActionReactive>(
                    typeof(InputActionReactiveFactory),
                    new MethodBindings())!;
            }

            return MethodInvoker.Invoke<IInputActionReactive>(
                typeof(InputActionReactiveFactory), 
                new MethodBindings
                {
                    GenericArguments = new Type[] { valueType }
                })!;
        }
    }
}
