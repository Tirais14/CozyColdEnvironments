using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem
{
    public class InputActionX : IInputAction
    {
        protected bool disposedValue;

        protected readonly InputAction inputAction;

        public bool IsButtonPressed => inputAction.IsPressed();

        public event Action<CallbackContext>? OnStarted;
        public event Action<CallbackContext>? OnPerformed;
        public event Action<CallbackContext>? OnCanceled;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public InputAction AsUnityInputAction() => inputAction;

        public InputActionX(InputAction inputAction)
        {
            this.inputAction = inputAction;

            this.inputAction.started += StartedEvent;
            this.inputAction.performed += PerformedEvent;
            this.inputAction.canceled += CanceledEvent;
        }

        protected virtual void DisposeManaged()
        {
            inputAction.started -= StartedEvent;
            inputAction.performed -= PerformedEvent;
            inputAction.canceled -= CanceledEvent;
        }

        protected virtual void DisposeOther()
        {
            OnStarted = null;
            OnPerformed = null;
            OnCanceled = null;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    DisposeManaged();

                DisposeOther();

                disposedValue = true;
            }
        }

        private void StartedEvent(CallbackContext context)
        {
            OnStarted?.Invoke(context);
        }

        private void PerformedEvent(CallbackContext context)
        {
            OnPerformed?.Invoke(context);
        }

        private void CanceledEvent(CallbackContext context)
        {
            OnCanceled?.Invoke(context);
        }

        public static explicit operator InputAction(InputActionX inputActionX)
        {
            return inputActionX.inputAction;
        }
    }
    public class InputActionX<T> : InputActionX, IInputAction<T>
        where T : struct
    {
        private T value;

        public T Value {
            get
            {
                if (disposedValue)
                    throw new Exception(inputAction.name + " is disposed, value cannot be readed.");

                return value;
            }
        }

        public event Action<T>? ValueOnStarted;
        public event Action<T>? ValueOnPerformed;
        public event Action<T>? ValueOnCanceled;

        public InputActionX(InputAction inputAction) : base(inputAction)
        {
            inputAction.started += ValueStartedEvent;
            inputAction.performed += ValuePerformedEvent;
            inputAction.canceled += ValueCanceledEvent;
        }

        protected virtual T ReadValue(CallbackContext context)
        {
            return context.ReadValue<T>();
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            inputAction.started -= ValueStartedEvent;
            inputAction.performed -= ValuePerformedEvent;
            inputAction.canceled -= ValueCanceledEvent;
        }

        protected override void DisposeOther()
        {
            base.DisposeOther();

            ValueOnStarted = null;
            ValueOnPerformed = null;
            ValueOnCanceled = null;
        }

        private void ValueStartedEvent(CallbackContext context)
        {
            value = ReadValue(context);

            ValueOnStarted?.Invoke(value);
        }

        private void ValuePerformedEvent(CallbackContext context)
        {
            value = ReadValue(context);

            ValueOnPerformed?.Invoke(value);
        }

        private void ValueCanceledEvent(CallbackContext context)
        {
            value = ReadValue(context);

            ValueOnCanceled?.Invoke(value);
        }
    }
}
