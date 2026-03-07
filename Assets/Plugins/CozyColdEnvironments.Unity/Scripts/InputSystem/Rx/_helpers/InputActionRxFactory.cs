using System;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public static class InputActionRxFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static T Create<T>(InputAction inputAction)
            where T : IInputActionRx
        {
            return Create(typeof(T), inputAction).To<T>();
        }
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IInputActionRx Create(Type type, InputAction inputAction)
        {
            Guard.IsNotNull(type);
            Guard.IsNotNull(inputAction);

            return type.Reflect()
                       .WithArguments(inputAction)
                       .CreateInstance<IInputActionRx>();
        }
    }
}
