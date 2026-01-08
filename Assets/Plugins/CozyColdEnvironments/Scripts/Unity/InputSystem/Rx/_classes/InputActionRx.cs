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
        private readonly ReactiveCommand<CallbackContext> raw = new();
        private readonly ReactiveCommand<CallbackContext> started = new();
        private readonly ReactiveCommand<CallbackContext> performed = new();
        private readonly ReactiveCommand<CallbackContext> canceled = new();

        private bool disposed;

        public InputAction Action { get; }
        public Observable<CallbackContext> Raw => raw;
        public Observable<CallbackContext> Started {
            get => started;
        }
        public Observable<CallbackContext> Performed {
            get => performed;
        }
        public Observable<CallbackContext> Canceled {
            get => canceled;
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

                raw.Dispose();
                started.Dispose();
                performed.Dispose();
                canceled.Dispose();
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
        protected readonly List<IDisposable> disposables = new();

        public T InputValue { get; private set; }
        public virtual Observable<T> TRaw { get; }
        public virtual Observable<T> TStarted { get; }
        public virtual Observable<T> TPerformed { get; }
        public virtual Observable<T> TCanceled { get; }

        public InputActionRx(InputAction inputAction) 
            :
            base(inputAction)
        {
            TRaw = Raw.Select(x => x.ReadValue<T>());
            TStarted = Started.Select(x => x.ReadValue<T>());
            TPerformed = Performed.Select(x => x.ReadValue<T>());
            TCanceled = Canceled.Select(x => x.ReadValue<T>());

            TStarted.Subscribe(x => InputValue = x).AddTo(disposables);
            TPerformed.Subscribe(x => InputValue = x).AddTo(disposables);
            TCanceled.Subscribe(x => InputValue = x).AddTo(disposables);
        }
    }
}
