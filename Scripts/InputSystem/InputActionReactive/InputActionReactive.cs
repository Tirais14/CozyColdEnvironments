using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Disposables;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.InputSystem.Reactive
{
    public class InputActionReactive 
        :
        DisposableContainer,
        IInputActionReactive
    {
        private readonly ReactiveProperty<CallbackContext> trigger = new();
        private readonly ReactiveProperty<CallbackContext> started = new();
        private readonly ReactiveProperty<CallbackContext> performed = new();
        private readonly ReactiveProperty<CallbackContext> canceled = new();

        private bool disposedValue;

        public InputAction Action { get; }
        public IObservable<CallbackContext> Trigger => trigger.AsObservable();
        public IObservable<CallbackContext> Started {
            get => started.Where(x => !x.Equals(default(CallbackContext)));
        }
        public IObservable<CallbackContext> Performed {
            get => performed.Where(x => !x.Equals(default(CallbackContext)));
        }
        public IObservable<CallbackContext> Canceled {
            get => canceled.Where(x => !x.Equals(default(CallbackContext)));
        }
        public IObservable<bool> ButtonStarted {
            get => started.Select(x => x.ReadValueAsButton());
        }
        public IObservable<bool> ButtonPerformed {
            get => performed.Select(x => x.ReadValueAsButton());
        }
        public IObservable<bool> ButtonCanceled {
            get => canceled.Select(x => x.ReadValueAsButton());
        }
        public string ActionName => Action.name;
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

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Action.started -= OnTrigger;
                    Action.performed -= OnTrigger;
                    Action.canceled -= OnTrigger;

                    Action.started -= OnStarted;
                    Action.performed -= OnPerformed;
                    Action.canceled -= OnCanceled;
                }

                disposedValue = true;
            }
        }

        private void Setup()
        {
            Action.started += OnTrigger;
            Action.performed += OnTrigger;
            Action.canceled += OnTrigger;

            Action.started += OnStarted;
            Action.performed += OnPerformed;
            Action.canceled += OnCanceled;
        }

        private void OnTrigger(CallbackContext context)
        {
            trigger.Value = context;
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
    }
    public class InputActionReactive<T> 
        :
        InputActionReactive,
        IInputActionReactive<T>

        where T : struct
    {
        public T Value { get; private set; }
        public IObservable<T> TStarted => Started.Select(x => x.ReadValue<T>());
        public IObservable<T> TPerformed => Started.Select(x => x.ReadValue<T>());
        public IObservable<T> TCanceled => Started.Select(x => x.ReadValue<T>());

        public InputActionReactive(InputAction inputAction) 
            :
            base(inputAction)
        {
            TStarted.Subscribe(x => Value = x).AddTo(this);
            TPerformed.Subscribe(x => Value = x).AddTo(this);
            TCanceled.Subscribe(x => Value = x).AddTo(this);
        }
    }
}
