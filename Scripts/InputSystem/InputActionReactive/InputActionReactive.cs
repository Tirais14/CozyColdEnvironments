using System;
using UniRx;
using UnityEngine.InputSystem;
using UTIRLib.Disposables;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
{
    public class InputActionReactive 
        :
        IInputActionReactive
    {
        private readonly ReactiveProperty<CallbackContext> started = new();
        private readonly ReactiveProperty<CallbackContext> performed = new();
        private readonly ReactiveProperty<CallbackContext> canceled = new();

        private bool disposedValue;

        public InputAction Action { get; }
        public IObservable<CallbackContext> Started {
            get => started.Where(x => !x.Equals(default(CallbackContext)));
        }
        public IObservable<CallbackContext> Performed {
            get => performed.Where(x => !x.Equals(default(CallbackContext)));
        }
        public IObservable<CallbackContext> Canceled {
            get => canceled.Where(x => !x.Equals(default(CallbackContext)));
        }
        public bool IsEnabled => Action.enabled;

        public InputActionReactive(InputAction inputAction)
        {
            Action = inputAction;  
            Setup();
        }

        public static explicit operator InputAction(InputActionReactive inputActionReactive)
        {
            return inputActionReactive.Action;
        }

        public bool IsButtonPressed() => Action.IsPressed();

        public void Enable() => Action.Enable();

        public void Disable() => Action.Disable();

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Setup()
        {
            Action.started += OnStarted;
            Action.performed += OnPerformed;
            Action.canceled += OnCanceled;
        }

        private void OnStarted(CallbackContext context)
        {
            started.Value = context;
        }

        private void OnPerformed(CallbackContext context)
        {
            performed.Value = context;
        }

        private void OnCanceled(CallbackContext context)
        {
            canceled.Value = context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Action.started -= OnStarted;
                    Action.performed -= OnPerformed;
                    Action.canceled -= OnCanceled;
                }

                disposedValue = true;
            }
        }
    }
    public class InputActionReactive<T> 
        :
        InputActionReactive,
        IInputActionReactive<T>

        where T : struct
    {
        private readonly ReactiveProperty<T> startedT = new();
        private readonly ReactiveProperty<T> performedT = new();
        private readonly ReactiveProperty<T> canceledT = new();
        private bool disposedValue;

        public T Value { get; private set; }
        public IObservable<T> StartedT => startedT.AsObservable();
        public IObservable<T> PerformedT => performedT.AsObservable();
        public IObservable<T> CanceledT => canceledT.AsObservable();

        public InputActionReactive(InputAction inputAction) 
            :
            base(inputAction)
        {
            Setup();
        }

        protected virtual T GetInputValue(CallbackContext context)
        {
            return context.ReadValue<T>();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    Action.started -= OnStarted;
                    Action.performed -= OnPerformed;
                    Action.canceled -= OnCanceled;
                }

                disposedValue = true;
            }
        }

        private void Setup()
        {
            Action.started += OnStarted;
            Action.performed += OnPerformed;
            Action.canceled += OnCanceled;
        }

        private void OnStarted(CallbackContext context)
        {
            Value = GetInputValue(context);
            startedT.Value = Value;
        }

        private void OnPerformed(CallbackContext context)
        {
            Value = GetInputValue(context);
            performedT.Value = Value;
        }

        private void OnCanceled(CallbackContext context)
        {
            Value = GetInputValue(context);
            canceledT.Value = Value;
        }
    }
}
