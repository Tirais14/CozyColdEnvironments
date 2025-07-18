using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UTIRLib;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.InputSystem
{
    public static class InputActionFactory
    {
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

        /// <exception cref="ArgumentNullException"></exception>
        public static IReadOnlyDictionary<string, IInputAction<T>> Create<T>(
            InputActionMap actionMap,
            params string[] actionNames)
            where T : struct
        {
            if (actionMap is null)
                throw new ArgumentNullException(nameof(actionMap));
            if (actionNames.IsEmpty())
                return new Dictionary<string, IInputAction<T>>(0);

            var inputs = new Dictionary<string, IInputAction<T>>(actionNames.Length);

            IInputAction<T> input;
            for (int i = 0; i < actionNames.Length; i++)
            {
                input = Create<T>(actionMap, actionNames[i]);

                inputs.Add(actionNames[i], input);
            }

            return inputs;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        public static IReadOnlyDictionary<string, IInputAction<T>> Create<T>(
            InputActionAsset inputActions,
            string actionMapName,
            params string[] actionNames)
            where T : struct
        {
            if (inputActions == null)
                throw new ArgumentNullException(nameof(inputActions));
            if (actionMapName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(actionMapName), actionMapName);

            InputActionMap actionMap = inputActions.FindActionMap(actionMapName,
                                                                  throwIfNotFound: true);

            return Create<T>(actionMap, actionNames);
        }
    }
}
