using CCEnvs.Disposables;
using CCEnvs.Services;
using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.UnityX.InputSystem.Rx
{
    public class InputActionRx
        :
        IInputActionRx
    {
        private readonly CancellationTokenSource disposeCancellationTokeSource = new();

        private readonly ReactiveCommand<CallbackContext> raw = new();
        private readonly ReactiveCommand<CallbackContext> started = new();
        private readonly ReactiveCommand<CallbackContext> performed = new();
        private readonly ReactiveCommand<CallbackContext> canceled = new();

        private readonly ReactiveProperty<bool> isEnabled;

        private readonly Observable<Unit> preUpdate;

        private IDisposable? preUpdateBinding;

        public InputAction Action { get; }

        public string Name => Action.name;

        public bool IsEnabled => isEnabled.Value && Action.enabled;
        public bool IsHolding { get; private set; }
        public bool WasStartedOnThisFrame { get; private set; }
        public bool WasPerformedOnThisFrame { get; private set; }
        public bool WasCanceledOnThisFrame { get; private set; }

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokeSource.Token;

        [Preserve]
        public InputActionRx(InputAction inputAction)
        {
            CC.Guard.IsNotNull(inputAction, nameof(inputAction));

            isEnabled = new ReactiveProperty<bool>(inputAction.enabled);
            preUpdate = Observable.EveryUpdate(UnityFrameProvider.Initialization, DisposeCancellationToken)
                .Where(this, (_, @this) => @this.IsEnabled);

            Action = inputAction;

            Setup();
            BindPreUpdate();

            CCServices.Bind(GetType())
                .FromInstance(this)
                .WithID(Name)
                .WithInterfaces(nameof(IInputActionRx))
                .IfNotBound()
                .AsSingle();
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
            return performed;
        }

        public Observable<CallbackContext> ObserveCanceled()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            return canceled;
        }

        protected virtual void OnRaw(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            raw.Execute(context);
        }

        protected virtual void OnStarted(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            WasStartedOnThisFrame = true;
            IsHolding = true;

            started.Execute(context);
        }

        protected virtual void OnPerformed(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            WasPerformedOnThisFrame = true;

            performed.Execute(context);
        }

        protected virtual void OnCanceled(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            WasCanceledOnThisFrame = true;
            IsHolding = false;

            canceled.Execute(context);
        }

        protected virtual void OnPreUpdate(Unit _)
        {
            WasStartedOnThisFrame = false;
            WasPerformedOnThisFrame = false;
            WasCanceledOnThisFrame = false;
        }

        private int disposed;
        public void Dispose() => Dispose(disposing: true);
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
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

                CCDisposable.Dispose(ref preUpdateBinding);
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

        private void BindPreUpdate()
        {
            preUpdateBinding = preUpdate.Subscribe(OnPreUpdate);
        }
    }
    public class InputActionRx<T>
        :
        InputActionRx,
        IInputActionRx<T>

        where T : struct
    {
        private IDisposable? rawValueBinding;

        public T OnStartedValue { get; private set; }
        public T OnPerformedValue { get; private set; }
        public T OnCanceledValue { get; private set; }

        [Preserve]
        public InputActionRx(InputAction inputAction)
            :
            base(inputAction)
        {
            BindRawValue();
        }

        public T ReadValue() => Action.ReadValue<T>();

        public virtual Observable<T> ObserveRawValue()
        {
            return ObserveRaw().Select(this, static (ctx, @this) => @this.ReadValue(ctx));
        }

        public virtual Observable<T> ObserveStartedValue()
        {
            return ObserveStarted().Select(this, static (ctx, @this) => @this.OnStartedValue);
        }

        public virtual Observable<T> ObservePerformedValue()
        {
            return ObservePerformed().Select(this, static (ctx, @this) => @this.OnPerformedValue);
        }

        public virtual Observable<T> ObserveCanceledValue()
        {
            return ObserveCanceled().Select(this, static (ctx, @this) => @this.OnCanceledValue);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Name), Name)
                .AddProperty(nameof(IsEnabled), IsEnabled)
                .ToStringAndDispose();
        }

        protected virtual T ReadValue(CallbackContext context)
        {
            return context.ReadValue<T>();
        }

        protected override void OnStarted(CallbackContext context)
        {
            OnStartedValue = ReadValue(context);
            base.OnStarted(context);
        }

        protected override void OnPerformed(CallbackContext context)
        {
            OnPerformedValue = ReadValue(context);
            base.OnPerformed(context);
        }

        protected override void OnCanceled(CallbackContext context)
        {
            OnCanceledValue = ReadValue(context);
            base.OnCanceled(context);
        }

        protected override void OnPreUpdate(Unit _)
        {
            base.OnPreUpdate(_);
            OnStartedValue = default;
            OnPerformedValue = default;
            OnCanceledValue = default;
        }

        private int disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
                CCDisposable.Dispose(ref rawValueBinding);
        }

        private void OnRawValueChanged(T value)
        {
            OnPerformedValue = value;
        }

        private void BindRawValue()
        {
            rawValueBinding = ObserveRawValue().Subscribe(OnRawValueChanged);
        }
    }
}
