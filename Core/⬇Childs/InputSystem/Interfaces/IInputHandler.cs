using System;
using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public interface IInputHandler
    {
        void BindAction(string inputActionName, Action<InputAction.CallbackContext> action,
            InputActionEventType eventType = InputActionEventType.OnPerformed);

        void BindAction(string inputActionName, Action action, InputActionEventType eventType = InputActionEventType.OnPerformed);

        void UnbindAction(string inputActionName, Action<InputAction.CallbackContext> action, InputActionEventType eventType);

        void UnbindAction(string inputActionName, Action action, InputActionEventType eventType);

        bool IsButtonPressed(string inputActionName);

        InputAction GetInputAction(string inputActionName);

        T GetValueT<T>(string inputActionName) where T : struct;
    }

    public interface IInputHandler<T> : IInputHandler where T : struct
    {
        void BindAction(string inputActionName, Action<T> action, InputActionEventType eventType = InputActionEventType.OnPerformed);

        void UnbindAction(string inputActionName, Action<T> action, InputActionEventType eventType);

        T GetValue(string inputActionName);

        void GetValue(string inputActionName, out T value);
    }
}