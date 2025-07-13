using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.InputSystem
{
    public abstract class InputHandler : MonoX, IInputHandler
    {
        protected readonly Dictionary<Type, Type> handlerActionTypes = new();
        protected readonly Dictionary<string, InputAction> inputActions = new();
        protected readonly Dictionary<string, IInputHandlerAction> handlerActions = new();
        [SerializeField] protected InputActionAsset inputActionAsset = null!;

        public void BindAction(string inputActionName, Action<InputAction.CallbackContext> action,
           InputActionEventType eventType = InputActionEventType.OnPerformed) =>
            BindActionInternal(inputActionName, action, eventType);

        public void BindAction(string inputActionName, Action action,
            InputActionEventType eventType = InputActionEventType.OnPerformed) =>
            BindActionInternal(inputActionName, action, eventType);

        public void UnbindAction(string inputActionName, Action<InputAction.CallbackContext> action, InputActionEventType eventType) =>
            UnbindActionInternal(inputActionName, action, eventType);

        public void UnbindAction(string inputActionName, Action action, InputActionEventType eventType) =>
            UnbindActionInternal(inputActionName, action, eventType);

        public bool IsButtonPressed(string inputActionName) => GetInputAction(inputActionName).IsPressed();

        /// <exception cref="ArgumentException"></exception>
        public InputAction GetInputAction(string inputActionName)
        {
            if (!inputActions.TryGetValue(inputActionName, out InputAction inputAction))
            {
                throw new ArgumentException(nameof(inputActionName));
            }

            return inputAction;
        }

        public T GetValueT<T>(string inputActionName) where T : struct => GetInputAction(inputActionName).ReadValue<T>();

        protected void BindActionInternal(string inputActionName, Delegate action,
            InputActionEventType eventType = InputActionEventType.OnPerformed)
        {
            if (string.IsNullOrEmpty(inputActionName))
            {
                throw new StringArgumentException(inputActionName, nameof(inputActionName));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (handlerActions.TryGetValue(inputActionName, out IInputHandlerAction inputHandlerItem))
            {
                inputHandlerItem.Subscribe(action, eventType);
            }
            else if (inputActions.TryGetValue(inputActionName, out InputAction inputAction) &&
                !handlerActions.ContainsKey(inputAction.name))
            {
                AddHandlerAction(inputAction);
                handlerActions[inputAction.name].Subscribe(action, eventType);
            }
            else
            {
                throw new InputActionNotFoundException(inputActionName);
            }
        }

        /// <exception cref="NullOrEmptyStringException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        protected void UnbindActionInternal(string inputActionName, Delegate action, InputActionEventType eventType)
        {
            if (string.IsNullOrEmpty(inputActionName))
            {
                throw new StringArgumentException(inputActionName, nameof(inputActionName));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (handlerActions.TryGetValue(inputActionName, out IInputHandlerAction inputHandlerItem))
            {
                inputHandlerItem.Unsubscribe(action, eventType);
            }
            else
            {
                throw new InputActionNotFoundException(inputActionName);
            }
        }

        protected bool IsInputActionExists(string inputActionName)
        {
            foreach (var registeredInputActionName in inputActions.Keys)
            {
                if (registeredInputActionName == inputActionName)
                {
                    return true;
                }
            }

            return false;
        }

        protected void AddHandlerAction(InputAction inputAction)
        {
            if (!handlerActionTypes.TryGetValue(InputActionHelper.GetValueType(inputAction), out Type handlerActionType))
            {
                Debug.LogWarning($"{inputAction.name} control type: {inputAction.expectedControlType} doesn't supported");
                return;
            }

            ConstructorInfo handlerActionConstructor = handlerActionType.GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                binder: null, new Type[] { typeof(InputAction) }, Array.Empty<ParameterModifier>());

            IInputHandlerAction handlerAction = (handlerActionConstructor.Invoke(new object[] { inputAction }) as IInputHandlerAction)
                ?? throw new NullReferenceException();
            handlerActions.Add(inputAction.name, (handlerAction));
            SubscribeInputHandlerItem(handlerAction, inputAction);
        }

        protected void AddHandlerActionsByMap(string inputActionMapName)
        {
            InputActionMap actionMap = inputActionAsset.FindActionMap(inputActionMapName);
            if (actionMap != null)
            {
                foreach (InputAction inputAction in actionMap.actions)
                {
                    inputActions.Add(inputAction.name, inputAction);
                }
            }
            else
            {
                Debug.LogWarning($"Input action map {inputActionMapName} doesn't found.");
                return;
            }

            foreach (InputAction inputAction in inputActions.Values)
            {
                AddHandlerAction(inputAction);
            }
        }

        protected virtual void UnregisterInputActions()
        {
            foreach (InputAction inputAction in inputActions.Values)
            {
                UnregisterInputAction(inputAction);
            }
        }

        protected virtual void UnregisterInputAction(InputAction inputAction) => handlerActions.Remove(inputAction.name);

        protected void SetHandlerActionTypes(KeyValuePair<Type, Type>[]? handlerActionTypes = null)
        {
            if (handlerActionTypes.IsNullOrEmpty())
            {
                handlerActionTypes = new KeyValuePair<Type, Type>[] {
                    new(typeof(bool), typeof(ButtonInputHandlerAction)),
                    new(typeof(Vector2), typeof(Vector2InputHandlerAction)),
                    new(typeof(Vector3), typeof(Vector3InputHandlerAction)),
                    new(typeof(Quaternion), typeof(QuaternionInputHandlerAction)),
                };
            }

            handlerActionTypes.ForEach((pair) => this.handlerActionTypes.Add(pair.Key, pair.Value));
        }

        private static void SubscribeInputHandlerItem(IInputHandlerAction inputHandlerItem, InputAction inputAction)
        {
            inputAction.performed += inputHandlerItem.OnPerformedAction;
            inputAction.started += inputHandlerItem.OnStartedAction;
            inputAction.canceled += inputHandlerItem.OnCancelledAction;
        }
    }
}