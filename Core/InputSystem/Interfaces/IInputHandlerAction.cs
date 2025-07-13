using System;
using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public interface IInputHandlerAction
    {
        event Action<InputAction.CallbackContext> OnPerformedRaw;

        event Action<InputAction.CallbackContext> OnStartedRaw;

        event Action<InputAction.CallbackContext> OnCancelledRaw;

        event Action OnPerformed;

        event Action OnStarted;

        event Action OnCancelled;

        void Subscribe(Delegate action, InputActionEventType eventType);

        void Unsubscribe(Delegate action, InputActionEventType eventType);

        void OnPerformedAction(InputAction.CallbackContext context);

        void OnStartedAction(InputAction.CallbackContext context);

        void OnCancelledAction(InputAction.CallbackContext context);
    }

    public interface IInputHandlerAction<TInputValue> : IInputHandlerAction
        where TInputValue : struct
    {
        event Action<TInputValue> OnPerformedWithValue;

        event Action<TInputValue> OnStartedWithValue;

        event Action<TInputValue> OnCancelledWithValue;

        TInputValue Value { get; }
    }
}