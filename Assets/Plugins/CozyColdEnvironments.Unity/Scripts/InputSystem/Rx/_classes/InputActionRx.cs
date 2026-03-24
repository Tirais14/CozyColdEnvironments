using System;
using System.Collections.Generic;
using System.Threading;
using CCEnvs.Disposables;
using CCEnvs.Rx;
using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
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

        private readonly CancellationTokenSource disposeCancellationTokeSource = new();

        private readonly ReactiveCommand<CallbackContext> raw = new();
        private readonly ReactiveCommand<CallbackContext> started = new();
        private readonly ReactiveCommand<CallbackContext> performed = new();
        private readonly ReactiveCommand<CallbackContext> canceled = new();

        private readonly ReactiveProperty<bool> isEnabled;

        //private Observable<Unit>? everyUpdateThread;

        //private bool performedState;

        public InputAction Action { get; }

        public string ActionName => Action.name;

        public bool IsEnabled => isEnabled.Value && Action.enabled;
        public bool IsHolding { get; private set; }

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokeSource.Token;

        [Preserve]
        public InputActionRx(InputAction inputAction)
        {
            CC.Guard.IsNotNull(inputAction, nameof(inputAction));

            isEnabled = new ReactiveProperty<bool>(inputAction.enabled);

            Action = inputAction;
            Setup();
        }

        ~InputActionRx() => Dispose();

        public static explicit operator InputAction(InputActionRx inputActionReactive)
        {
            return inputActionReactive.Action;
        }

        public bool IsButtonPressed() => Action.IsPressed();

        public T ReadValue<T>()
            where T : struct
        {
            return Action.ReadValue<T>();
        }

        public void Enable()
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            Action.Enable();
            isEnabled.Value = true;
        }

        public void Disable()
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            Action.Disable();
            isEnabled.Value = false;
        }

        public Observable<bool> ObserveEnabled()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            return isEnabled.Where(static x => x);
        }

        public Observable<bool> ObserveDisabled()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            return isEnabled.Where(static x => !x);
        }

        public Observable<CallbackContext> ObserveRaw()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            return raw;
        }

        public Observable<CallbackContext> ObserveStarted()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            return started;
        }

        public Observable<CallbackContext> ObservePerformed()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            //performedState = true;

            return performed;
        }

        public Observable<CallbackContext> ObserveCanceled()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            //performedState = false;

            return canceled;
        }

        //public Observable<Unit> ObserveIsPressed()
        //{
        //    CCDisposable.ThrowIfDisposed(this, disposed);

        //    everyUpdateThread ??= Observable.EveryUpdate(UnityFrameProvider.EarlyUpdate);

        //    return everyUpdateThread.Where(Action, static (_, action) => action.IsPressed());
        //}

        //public Observable<Unit> ObserveContinuousPerformed()
        //{
        //    CCDisposable.ThrowIfDisposed(this, disposed);

        //    return ObserveIsPressed().Where(Action, (_, action) => action.WasPerformedThisFrame());
        //}

        private int disposed;

        public void Dispose() => Dispose(disposing: true);
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)

            if (disposing)
            {
                disposables.DisposeEachAndClear();

                disposeCancellationTokeSource.CancelAndDispose();

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

            GC.SuppressFinalize(this);
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
            if (CCDisposable.IsDisposed(disposed))
                return;

            raw.Execute(context);
        }

        private void OnStarted(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            started.Execute(context);
            IsHolding = true;
        }

        private void OnPerformed(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            performed.Execute(context);
        }

        private void OnCanceled(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            canceled.Execute(context);
            IsHolding = false;
        }
    }
    public class InputActionRx<T>
        :
        InputActionRx,
        IInputActionRx<T>

        where T : struct
    {
        public T InputValue { get; private set; }

        [Preserve]
        public InputActionRx(InputAction inputAction)
            :
            base(inputAction)
        {
            ObserveRawValue().Subscribe(this,
                static (x, @this) => @this.InputValue = x)
                .AddTo(disposables);
        }

        public T ReadValue() => Action.ReadValue<T>();

        public virtual Observable<T> ObserveRawValue()
        {
            return ObserveRaw().Select(static x => x.ReadValue<T>());
        }

        public virtual Observable<T> ObserveStartedValue()
        {
            return ObserveStarted().Select(static x => x.ReadValue<T>());
        }

        public virtual Observable<T> ObservePerformedValue()
        {
            return ObservePerformed().Select(static x => x.ReadValue<T>());
        }

        public virtual Observable<T> ObserveCanceledValue()
        {
            return ObserveCanceled().Select(static x => x.ReadValue<T>());
        }
    }
}
