using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using CCEnvs.Disposables;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class InputActionRx 
        :
        DisposableContainer,
        IInputActionRx
    {
        private readonly ReactiveProperty<CallbackContext> raw = new();
        private readonly ReactiveProperty<CallbackContext> started = new();
        private readonly ReactiveProperty<CallbackContext> performed = new();
        private readonly ReactiveProperty<CallbackContext> canceled = new();

        private bool disposedValue;

        public InputAction Action { get; }
        public IObservable<CallbackContext> Raw => raw.AsObservable();
        public IObservable<CallbackContext> Started {
            get => started.Skip(1);
        }
        public IObservable<CallbackContext> Performed {
            get => performed.Skip(1);
        }
        public IObservable<CallbackContext> Canceled {
            get => canceled.Skip(1);
        }
        public IObservable<bool> ButtonRaw => raw.Select(x => x.ReadValueAsButton());
        public IObservable<bool> ButtonStarted {
            get => Started.Select(x => x.ReadValueAsButton());
        }
        public IObservable<bool> ButtonPerformed {
            get => Performed.Select(x => x.ReadValueAsButton());
        }
        public IObservable<bool> ButtonCanceled {
            get => Canceled.Select(x => x.ReadValueAsButton());
        }
        public string ActionName => Action.name;
        public bool IsEnabled => Action.enabled;

        public InputActionRx(InputAction inputAction)
        {
            CCEnvs.CC.Guard.NullArgument(inputAction, nameof(inputAction));

            Action = inputAction;  
            Setup();
        }

        public static explicit operator InputAction(InputActionRx inputActionReactive)
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
                    Action.started -= OnRaw;
                    Action.performed -= OnRaw;
                    Action.canceled -= OnRaw;

                    Action.started -= OnStarted;
                    Action.performed -= OnPerformed;
                    Action.canceled -= OnCanceled;
                }

                disposedValue = true;
            }
        }

        private void Setup()
        {
            Action.started += OnRaw;
            Action.performed += OnRaw;
            Action.canceled += OnRaw;

            Action.started += OnStarted;
            Action.performed += OnPerformed;
            Action.canceled += OnCanceled;
        }

        private void OnRaw(CallbackContext context)
        {
            raw.SetValueAndForceNotify(context);
        }

        private void OnStarted(CallbackContext context)
        {
            started.SetValueAndForceNotify(context);
        }

        private void OnPerformed(CallbackContext context)
        {
            performed.SetValueAndForceNotify(context);
        }

        private void OnCanceled(CallbackContext context)
        {
            canceled.SetValueAndForceNotify(context);
        }
    }
    public class InputActionRx<T> 
        :
        InputActionRx,
        IInputActionRx<T>

        where T : struct
    {
        public T Value { get; private set; }
        public IObservable<T> TRaw => Raw.Select(x => x.ReadValue<T>());
        public IObservable<T> TStarted => Started.Select(x => x.ReadValue<T>());
        public IObservable<T> TPerformed => Performed.Select(x => x.ReadValue<T>());
        public IObservable<T> TCanceled => Canceled.Select(x => x.ReadValue<T>());

        public InputActionRx(InputAction inputAction) 
            :
            base(inputAction)
        {
            TStarted.Subscribe(x => Value = x).AddTo(this);
            TPerformed.Subscribe(x => Value = x).AddTo(this);
            TCanceled.Subscribe(x => Value = x).AddTo(this);
        }
    }
}
