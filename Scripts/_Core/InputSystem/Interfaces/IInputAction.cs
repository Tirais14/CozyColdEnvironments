using System;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem
{
    public interface IInputAction : IStateSwitchable, IDisposable
    {
        bool IsButtonPressed { get; }

        InputActionSubscription SubscribeOnStarted(Action<CallbackContext> action);
        InputActionSubscription SubscribeOnStarted(Action action);

        InputActionSubscription SubscribeOnPerformed(Action<CallbackContext> action);
        InputActionSubscription SubscribeOnPerformed(Action action);

        InputActionSubscription SubscribeOnCanceled(Action<CallbackContext> action);
        InputActionSubscription SubscribeOnCanceled(Action action);

        InputActionSubscription Subscribe(Action<CallbackContext> action,
                                          InputActionSubscriptionType subscriptionType);
        InputActionSubscription Subscribe(Action action,
                                         InputActionSubscriptionType subscriptionType);

        void UnsubscribeOnStarted(Action<CallbackContext> action);
        void UnsubscribeOnStarted(Action action);

        void UnsubscribeOnPerformed(Action<CallbackContext> action);
        void UnsubscribeOnPerformed(Action action);

        void UnsubscribeOnCanceled(Action<CallbackContext> action);
        void UnsubscribeOnCanceled(Action action);

        void Unsubscribe(Action<CallbackContext> action,
                         InputActionSubscriptionType subscriptionType);
        void Unsubscribe(Action action,
                         InputActionSubscriptionType subscriptionType);
    }
    public interface IInputAction<T> : IInputAction
        where T : struct
    {
        T Value { get; }

        InputActionSubscription<T> SubscribeOnStarted(Action<T> action);

        InputActionSubscription<T> SubscribeOnPerformed(Action<T> action);

        InputActionSubscription<T> SubscribeOnCanceled(Action<T> action);

        InputActionSubscription<T> Subscribe(Action<T> action,
            InputActionSubscriptionType subscriptionType);

        void UnsubscribeOnStarted(Action<T> action);

        void UnsubscribeOnPerformed(Action<T> action);

        void UnsubscribeOnCanceled(Action<T> action);

        void Unsubscribe(Action<T> action,
                         InputActionSubscriptionType subscriptionType);
    }
}
