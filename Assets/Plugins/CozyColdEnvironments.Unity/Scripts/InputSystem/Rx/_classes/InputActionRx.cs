using System;
using System.Collections.Generic;
using R3;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
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

        private readonly ReactiveProperty<bool> isEnabled;

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

        public bool IsEnabled => isEnabled.Value && Action.enabled;

        [Preserve]
        public InputActionRx(InputAction inputAction)
        {
            CC.Guard.IsNotNull(inputAction, nameof(inputAction));

            isEnabled = new ReactiveProperty<bool>(inputAction.enabled);

            Action = inputAction;
            Setup();
        }

        public static explicit operator InputAction(InputActionRx inputActionReactive)
        {
            return inputActionReactive.Action;
        }

        public bool IsButtonPressed() => Action.IsPressed();

        public void Enable()
        {
            ValidateDisposed();

            Action.Enable();
            isEnabled.Value = true;
        }

        public void Disable()
        {
            ValidateDisposed();

            Action.Disable();
            isEnabled.Value = false;
        }

        public Observable<bool> ObserveEnabled()
        {
            ValidateDisposed();

            return isEnabled.Where(static x => x);
        }

        public Observable<bool> ObserveDisabled()
        {
            ValidateDisposed();

            return isEnabled.Where(static x => !x);
        }

        private bool disposed;

        public void Dispose() => Dispose(disposing: true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                disposables.DisposeEachAndClear();

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
                isEnabled.Dispose();
            }

            disposed = true;
        }

        protected void ValidateDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
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

        public virtual Observable<T> TRaw { get; }
        public virtual Observable<T> TStarted { get; }
        public virtual Observable<T> TPerformed { get; }
        public virtual Observable<T> TCanceled { get; }

        [Preserve]
        public InputActionRx(InputAction inputAction)
            :
            base(inputAction)
        {
            TRaw = Raw.Select(static x => x.ReadValue<T>());
            TStarted = Started.Select(static x => x.ReadValue<T>());
            TPerformed = Performed.Select(static x => x.ReadValue<T>());
            TCanceled = Canceled.Select(static x => x.ReadValue<T>());

            TRaw.Subscribe(x => InputValue = x).AddTo(disposables);
        }
    }
}
