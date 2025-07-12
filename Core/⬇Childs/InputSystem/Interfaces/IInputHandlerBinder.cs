using System;

namespace UTIRLib.InputSystem
{
    public interface IInputHandlerBinder
    {
    }

    public interface IInputHandlerBinder<TInputValue> : IInputHandlerBinder
        where TInputValue : struct
    {
        public void BindInput(string inputActionName, Action<TInputValue> action, InputActionEventType eventType);

        public void UnbindInput(string inputActionName, Action<TInputValue> action, InputActionEventType eventType);

        public void UnbindAllInputs();
    }

    public interface IInputHandlerBinder<TInputValue, TEnum> : IInputHandlerBinder
    where TInputValue : struct
        where TEnum : Enum
    {
        public void BindInput(TEnum inputActionName, Action<TInputValue> action, InputActionEventType eventType);

        public void UnbindInput(TEnum inputActionName, Action<TInputValue> action, InputActionEventType eventType);

        public void UnbindAllInputs();
    }
}