using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem
{
    public class InputActionX : IInputAction
    {
        protected readonly InputAction inputAction;
        protected bool disposedValue;

        public event Action<CallbackContext>? OnPerformed;

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

            this.inputAction.performed += OnPerformedEvent;
        }

        protected virtual void DisposeManaged()
        {
            inputAction.performed -= OnPerformedEvent;
        }

        protected virtual void DisposeOther()
        {
            OnPerformed = null;
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

        private void OnPerformedEvent(CallbackContext context)
        {
            OnPerformed?.Invoke(context);
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

        public event Action<T>? OnPerformedValue;

        public InputActionX(InputAction inputAction) : base(inputAction)
        {
            base.inputAction.performed += SetValue;
            
            base.inputAction.performed += PerformedValueEvent;
        }

        protected virtual T ReadValue(CallbackContext context)
        {
            return context.ReadValue<T>();
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            inputAction.performed -= SetValue;

            inputAction.performed -= PerformedValueEvent;
        }

        protected override void DisposeOther()
        {
            base.DisposeOther();

            OnPerformedValue = null;
        }

        private void SetValue(CallbackContext context)
        {
            value = ReadValue(context);
        }

        private void PerformedValueEvent(CallbackContext context)
        {
            OnPerformedValue?.Invoke(ReadValue(context));
        }
    }
}
