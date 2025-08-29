using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine;
using UnityEngine.VFX;

#nullable enable
#pragma warning disable S3881
namespace CozyColdEnvironments.InputSystem
{
    public class InputActionX : IInputAction
    {
        protected readonly InputAction inputAction;
        protected bool disposedValue;

        protected Action<CallbackContext>? OnStarted;
        protected Action? OnStartedBasic;

        protected Action<CallbackContext>? OnPerformed;
        protected Action? OnPerformedBasic;

        protected Action<CallbackContext>? OnCanceled;
        protected Action? OnCanceledBasic;

        public bool IsButtonPressed => inputAction.IsPressed();
        public bool IsEnabled => inputAction.enabled;

        public InputActionX(InputAction inputAction)
        {
            this.inputAction = inputAction;

            this.inputAction.started += StartedEvent;
            this.inputAction.performed += PerformedEvent;
            this.inputAction.canceled += CanceledEvent;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription SubscribeOnStarted(Action<CallbackContext> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnStarted += action;

            return new InputActionSubscription(this,
                                               action,
                                               InputActionSubscriptionType.OnStarted);
        }
        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription SubscribeOnStarted(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnStartedBasic += action;

            return new InputActionSubscription(this,
                                               action,
                                               InputActionSubscriptionType.OnStarted);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription SubscribeOnPerformed(Action<CallbackContext> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnPerformed += action;

            return new InputActionSubscription(this,
                                               action,
                                               InputActionSubscriptionType.OnPerformed);
        }
        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription SubscribeOnPerformed(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnPerformedBasic += action;

            return new InputActionSubscription(this,
                                               action,
                                               InputActionSubscriptionType.OnPerformed);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription SubscribeOnCanceled(Action<CallbackContext> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnCanceled += action;

            return new InputActionSubscription(this,
                                               action,
                                               InputActionSubscriptionType.OnCanceled);
        }
        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription SubscribeOnCanceled(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnCanceledBasic += action;

            return new InputActionSubscription(this,
                                               action,
                                               InputActionSubscriptionType.OnCanceled);
        }

        public InputActionSubscription Subscribe(
            Action<CallbackContext> action,
            InputActionSubscriptionType subscriptionType)
        {
            return subscriptionType switch
            {
                InputActionSubscriptionType.OnStarted => SubscribeOnStarted(action),
                InputActionSubscriptionType.OnPerformed => SubscribeOnPerformed(action),
                InputActionSubscriptionType.OnCanceled => SubscribeOnCanceled(action),
                _ => throw new InvalidOperationException(subscriptionType.ToString()),
            };
        }

        public InputActionSubscription Subscribe(Action action, InputActionSubscriptionType subscriptionType)
        {
            return subscriptionType switch
            {
                InputActionSubscriptionType.OnStarted => SubscribeOnStarted(action),
                InputActionSubscriptionType.OnPerformed => SubscribeOnPerformed(action),
                InputActionSubscriptionType.OnCanceled => SubscribeOnCanceled(action),
                _ => throw new InvalidOperationException(subscriptionType.ToString()),
            };
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnStarted(Action<CallbackContext> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnStarted -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnStarted(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnStartedBasic -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnPerformed(Action<CallbackContext> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnPerformed -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnPerformed(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnPerformedBasic -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnCanceled(Action<CallbackContext> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnCanceled -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnCanceled(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            OnCanceledBasic -= action;
        }

        public void Unsubscribe(Action<CallbackContext> action,
                                InputActionSubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case InputActionSubscriptionType.OnStarted:
                    UnsubscribeOnStarted(action);
                    break;
                case InputActionSubscriptionType.OnPerformed:
                    UnsubscribeOnPerformed(action);
                    break;
                case InputActionSubscriptionType.OnCanceled:
                    UnsubscribeOnCanceled(action);
                    break;
                default:
                    throw new InvalidOperationException(subscriptionType.ToString());
            }
        }

        public void Unsubscribe(Action action,
                                InputActionSubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case InputActionSubscriptionType.OnStarted:
                    UnsubscribeOnStarted(action);
                    break;
                case InputActionSubscriptionType.OnPerformed:
                    UnsubscribeOnPerformed(action);
                    break;
                case InputActionSubscriptionType.OnCanceled:
                    UnsubscribeOnCanceled(action);
                    break;
                default:
                    throw new InvalidOperationException(subscriptionType.ToString());
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public InputAction AsUnityInputAction() => inputAction;

        public void Enable() => inputAction.Enable();

        public void Disable() => inputAction.Disable();

        protected virtual void StartedEvent(CallbackContext context)
        {
            OnStarted?.Invoke(context);

            OnStartedBasic?.Invoke();
        }

        protected virtual void PerformedEvent(CallbackContext context)
        {
            OnPerformed?.Invoke(context);

            OnPerformedBasic?.Invoke();
        }

        protected virtual void CanceledEvent(CallbackContext context)
        {
            OnCanceled?.Invoke(context);

            OnCanceledBasic?.Invoke();
        }

        protected virtual void DisposeManaged()
        {
            inputAction.started -= StartedEvent;
            inputAction.performed -= PerformedEvent;
            inputAction.canceled -= CanceledEvent;
        }

        protected virtual void DisposeOther()
        {
            OnStarted = null;
            OnPerformed = null;
            OnCanceled = null;
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

        public static explicit operator InputAction(InputActionX inputActionX)
        {
            return inputActionX.inputAction;
        }
    }
    public class InputActionX<T> : InputActionX, IInputAction<T>
        where T : struct
    {
        private T value;

        protected Action<T>? ValueOnStarted;
        protected Action<T>? ValueOnPerformed;
        protected Action<T>? ValueOnCanceled;

        public T Value {
            get
            {
                if (disposedValue)
                    throw new Exception(inputAction.name + " is disposed, value cannot be readed.");

                return value;
            }
        }

        public InputActionX(InputAction inputAction) : base(inputAction)
        {
            inputAction.started += ValueStartedEvent;
            inputAction.performed += ValuePerformedEvent;
            inputAction.canceled += ValueCanceledEvent;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription<T> SubscribeOnStarted(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ValueOnStarted += action;

            return new InputActionSubscription<T>(this,
                                                  action,
                                                  InputActionSubscriptionType.OnStarted);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription<T> SubscribeOnPerformed(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ValueOnPerformed += action;

            return new InputActionSubscription<T>(this,
                                                  action,
                                                  InputActionSubscriptionType.OnPerformed);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public InputActionSubscription<T> SubscribeOnCanceled(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ValueOnCanceled += action;

            return new InputActionSubscription<T>(this,
                                                  action,
                                                  InputActionSubscriptionType.OnCanceled);
        }

        public InputActionSubscription<T> Subscribe(Action<T> action,
            InputActionSubscriptionType subscriptionType)
        {
            return subscriptionType switch
            {
                InputActionSubscriptionType.OnStarted => SubscribeOnStarted(action),
                InputActionSubscriptionType.OnPerformed => SubscribeOnPerformed(action),
                InputActionSubscriptionType.OnCanceled => SubscribeOnCanceled(action),
                _ => throw new InvalidOperationException(subscriptionType.ToString()),
            };
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnStarted(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ValueOnStarted -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnPerformed(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ValueOnPerformed -= action;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void UnsubscribeOnCanceled(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            ValueOnCanceled -= action;
        }

        public void Unsubscribe(Action<T> action,
                                InputActionSubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case InputActionSubscriptionType.OnStarted:
                    UnsubscribeOnStarted(action);
                    break;
                case InputActionSubscriptionType.OnPerformed:
                    UnsubscribeOnPerformed(action);
                    break;
                case InputActionSubscriptionType.OnCanceled:
                    UnsubscribeOnCanceled(action);
                    break;
                default:
                    throw new InvalidOperationException(subscriptionType.ToString());
            }
        }

        protected virtual T ReadValue(CallbackContext context)
        {
            return context.ReadValue<T>();
        }

        protected virtual void ValueStartedEvent(CallbackContext context)
        {
            value = ReadValue(context);

            ValueOnStarted?.Invoke(value);
        }

        protected virtual void ValuePerformedEvent(CallbackContext context)
        {
            value = ReadValue(context);

            ValueOnPerformed?.Invoke(value);
        }

        protected virtual void ValueCanceledEvent(CallbackContext context)
        {
            value = ReadValue(context);

            ValueOnCanceled?.Invoke(value);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            inputAction.started -= ValueStartedEvent;
            inputAction.performed -= ValuePerformedEvent;
            inputAction.canceled -= ValueCanceledEvent;
        }

        protected override void DisposeOther()
        {
            base.DisposeOther();

            ValueOnStarted = null;
            ValueOnPerformed = null;
            ValueOnCanceled = null;
        }
    }
}
