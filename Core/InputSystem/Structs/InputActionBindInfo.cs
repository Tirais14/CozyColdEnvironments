using System;

#nullable enable

namespace UTIRLib.InputSystem
{
    public readonly struct InputActionBindInfo<TInputValue>
        where TInputValue : struct
    {
        private readonly string inputActionName;
        private readonly Action<TInputValue> action;
        private readonly InputActionEventType eventType;

        public readonly string InputActionName => inputActionName;
        public readonly Action<TInputValue> Action => action;
        public readonly InputActionEventType EventType => eventType;

        public InputActionBindInfo(string inputActionName, Action<TInputValue> action, InputActionEventType eventType)
        {
            this.inputActionName = inputActionName;
            this.action = action;
            this.eventType = eventType;
        }
    }
}