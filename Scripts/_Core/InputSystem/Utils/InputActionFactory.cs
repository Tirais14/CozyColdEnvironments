using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.InputSystem
{
    public static class InputActionFactory
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IInputAction Create(Type inputValueType,
                                          InputActionMap actionMap,
                                          string actionName)
        {
            if (inputValueType == null)
                throw new ArgumentNullException(nameof(inputValueType));
            if (inputValueType.IsNotAnyType(typeof(bool),
                                        typeof(Vector2),
                                        typeof(Vector3),
                                        typeof(Quaternion)))
                throw new ArgumentException($"Input value type cannot be {inputValueType.GetName()}.");

            MethodInfo createMethod = typeof(InputActionFactory).GetMethod(
                nameof(Create),
                genericParameterCount: 1,
                BindingFlagsDefault.StaticPublic,
                binder: null,
                new Type[] { typeof(InputActionMap), typeof(string) },
                Array.Empty<ParameterModifier>())
                    .MakeGenericMethod(inputValueType);

            return (IInputAction)createMethod.Invoke(obj: null,
                new object[] { actionMap, actionName });

        }
        public static IInputAction Create(Type inputValueType,
                                          InputActionAsset inputActions,
                                          string actionMapName,
                                          string actionName)
        {
            InputActionMap actionMap = inputActions.FindActionMap(actionMapName,
                                                                  throwIfNotFound: true);

            return Create(inputValueType, actionMap, actionName);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        public static IInputAction<T> Create<T>(InputActionMap actionMap,
                                                string actionName)
            where T : struct
        {
            if (actionMap is null)
                throw new ArgumentNullException(nameof(actionMap));
            if (actionName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(actionName), actionName);

            InputAction input = actionMap.FindAction(actionName, throwIfNotFound: true);

            if (typeof(T) == typeof(bool))
                return (new ButtonInputAction(input) as IInputAction<T>)!;

            return new InputActionX<T>(input);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        public static IInputAction<T> Create<T>(InputActionAsset inputActions,
                                                string actionMapName,
                                                string actionName)
            where T : struct
        {
            if (inputActions == null)
                throw new ArgumentNullException(nameof(inputActions));
            if (actionMapName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(actionMapName), actionMapName);
            if (actionName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(actionName), actionName);

            InputActionMap actionMap = inputActions.FindActionMap(actionMapName,
                                                           throwIfNotFound: true);

            return Create<T>(actionMap, actionName);
        }
    }
}
