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
        public static IInputAction Create(Type inputValueType,
                                          InputAction inputAction)
        {
            if (inputValueType is null)
                throw new ArgumentNullException(nameof(inputValueType));
            if (!IsValidType(inputValueType))
                throw new ArgumentException($"Input value type cannot be {inputValueType.GetName()}.");

            MethodInfo createMethod = typeof(InputActionFactory).GetMethod(
                nameof(Create),
                genericParameterCount: 1,
                BindingFlagsDefault.StaticPublic,
                binder: null,
                new Type[] { typeof(InputAction) },
                Array.Empty<ParameterModifier>())
                    .MakeGenericMethod(inputValueType);

            return (IInputAction)createMethod.Invoke(obj: null,
                new object[] { inputAction });
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IInputAction Create(Type inputValueType,
                                          InputActionMap actionMap,
                                          string actionName)
        {
            if (inputValueType is null)
                throw new ArgumentNullException(nameof(inputValueType));
            if (!IsValidType(inputValueType))
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
        public static IInputAction<T> Create<T>(InputAction inputAction)
            where T : struct
        {
            if (inputAction is null)
                throw new ArgumentNullException(nameof(inputAction));

            Type inputValueType = typeof(T);
            if (inputValueType == typeof(bool))
                return (IInputAction<T>)new ButtonInputAction(inputAction);
            else if (inputValueType == typeof(Vector2))
                return (IInputAction<T>)new Vector2InputAction(inputAction);

            return new InputActionX<T>(inputAction);
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

            return Create<T>(input);
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

        private static bool IsValidType(Type type)
        {
            if (!type.IsValueType)
                return false;
            if (type.IsNotAnyType(typeof(bool),
                                  typeof(float),
                                  typeof(double),
                                  typeof(int),
                                  typeof(Vector2),
                                  typeof(Vector3),
                                  typeof(Quaternion))
                )
                return false;

            return true;
        }
    }
}
