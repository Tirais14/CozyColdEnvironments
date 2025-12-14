using System;
using System.Collections.Generic;
using R3;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class InputActionRx 
        :
        IInputActionRx
    {
        protected readonly List<IDisposable> disposables = new();

        private readonly ReactiveCommand<CallbackContext> raw = new();
        private readonly ReactiveCommand<CallbackContext> started = new();
        private readonly ReactiveCommand<CallbackContext> performed = new();
        private readonly ReactiveCommand<CallbackContext> canceled = new();

        private bool disposed;

        public bool ButtonInputValue { get; private set; }
        public InputAction Action { get; }
        public Observable<CallbackContext> Raw => raw;
        public Observable<CallbackContext> Started {
            get => started.Skip(1);
        }
        public Observable<CallbackContext> Performed {
            get => performed.Skip(1);
        }
        public Observable<CallbackContext> Canceled {
            get => canceled.Skip(1);
        }
        public string ActionName => Action.name;
        public bool IsEnabled => Action.enabled;

        public InputActionRx(InputAction inputAction)
        {
            CCEnvs.CC.Guard.IsNotNull(inputAction, nameof(inputAction));

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

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Action.started -= OnRaw;
                Action.performed -= OnRaw;
                Action.canceled -= OnRaw;

                Action.started -= OnStarted;
                Action.performed -= OnPerformed;
                Action.canceled -= OnCanceled;
            }

            disposed = true;
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
            raw.Execute(context);
        }

        private void OnStarted(CallbackContext context)
        {
            started.Execute(context);
        }

        private void OnPerformed(CallbackContext context)
        {
            performed.Execute(context);
        }

        private void OnCanceled(CallbackContext context)
        {
            canceled.Execute(context);
        }
    }
    public class InputActionRx<T> 
        :
        InputActionRx,
        IInputActionRx<T>

        where T : struct
    {
        public T InputValue { get; private set; }
        public virtual Observable<T> TRaw => Raw.Select(x => x.ReadValue<T>());
        public virtual Observable<T> TStarted => Started.Select(x => x.ReadValue<T>());
        public virtual Observable<T> TPerformed => Performed.Select(x => x.ReadValue<T>());
        public virtual Observable<T> TCanceled => Canceled.Select(x => x.ReadValue<T>());

        public InputActionRx(InputAction inputAction) 
            :
            base(inputAction)
        {
            TStarted.Subscribe(x => InputValue = x).AddTo(disposables);
            TPerformed.Subscribe(x => InputValue = x).AddTo(disposables);
            TCanceled.Subscribe(x => InputValue = x).AddTo(disposables);
        }
    }
}
