using CCEnvs.Disposables;
using CCEnvs.Services;
using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
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
        protected struct InputValue
        {
            private long frame;

            public readonly bool IsSet => frame == Time.frameCount;

            public InputValue Set()
            {
                frame = Time.frameCount;
                return this;
            }
        }

        protected struct InputValue<T> where T : struct
        {
            private long frame;

            private T value;

            public readonly T Value {
                get
                {
                    if (!IsSet)
                        return default;

                    return value;
                }
            }

            public readonly bool IsSet => frame == Time.frameCount;

            public InputValue<T> Set(T value)
            {
                this.value = value;
                frame = Time.frameCount;
                return this;
            }
        }

        private readonly CancellationTokenSource disposeCancellationTokeSource = new();

        private readonly ReactiveCommand<CallbackContext> raw = new();
        private readonly ReactiveCommand<CallbackContext> started = new();
        private readonly ReactiveCommand<CallbackContext> performed = new();
        private readonly ReactiveCommand<CallbackContext> canceled = new();

        private readonly ReactiveProperty<bool> isEnabled;

        private InputValue wasStartedThisFrame;
        private InputValue wasPerformedThisFrame;
        private InputValue wasCanceledThisFrame;

        public InputAction Action { get; }

        public string Name => Action.name;

        public bool IsEnabled => isEnabled.Value && Action.enabled;
        public bool IsHolding { get; private set; }
        public bool WasStartedThisFrame => wasStartedThisFrame.IsSet;
        public bool WasPerformedThisFrame => wasPerformedThisFrame.IsSet;
        public bool WasCanceledThisFrame => wasCanceledThisFrame.IsSet;

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokeSource.Token;

        [Preserve]
        public InputActionRx(InputAction inputAction)
        {
            CC.Guard.IsNotNull(inputAction, nameof(inputAction));

            isEnabled = new ReactiveProperty<bool>(inputAction.enabled);

            Action = inputAction;

            Setup();

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

            wasStartedThisFrame.Set();
            IsHolding = true;

            started.Execute(context);
        }

        protected virtual void OnPerformed(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            wasPerformedThisFrame.Set();

            performed.Execute(context);
        }

        protected virtual void OnCanceled(CallbackContext context)
        {
            if (CCDisposable.IsDisposed(disposed))
                return;

            wasCanceledThisFrame.Set();
            IsHolding = false;

            canceled.Execute(context);
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
    }
    public class InputActionRx<T>
        :
        InputActionRx,
        IInputActionRx<T>

        where T : struct
    {
        private InputValue<T> onStartedValue;
        private InputValue<T> onPerformedValue;
        private InputValue<T> onCanceledValue;

        public T OnStartedValue => onStartedValue.Value;
        public T OnPerformedValue => onPerformedValue.Value;
        public T OnCanceledValue => onCanceledValue.Value;

        [Preserve]
        public InputActionRx(InputAction inputAction)
            :
            base(inputAction)
        {
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
            onStartedValue.Set(ReadValue(context));
            base.OnStarted(context);
        }

        protected override void OnPerformed(CallbackContext context)
        {
            onPerformedValue.Set(ReadValue(context));
            base.OnPerformed(context);
        }

        protected override void OnCanceled(CallbackContext context)
        {
            onCanceledValue.Set(ReadValue(context));
            base.OnCanceled(context);
        }
    }
}
