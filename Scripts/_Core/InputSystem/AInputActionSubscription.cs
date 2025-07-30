using System;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.InputSystem
{
    public abstract class AInputActionSubscription<TInput, TAction> : IDisposable
        where TInput : IInputAction
        where TAction : Delegate
    {
        protected readonly TAction action;

        public TInput InputAction { get; private set; }
        public InputActionSubscriptionType SubscriptionType { get; private set; }

        /// <exception cref="InvalidOperationException"></exception>
        protected AInputActionSubscription(TInput inputAction,
                                           TAction action,
                                           InputActionSubscriptionType subscriptionType)
        {
            if (subscriptionType == InputActionSubscriptionType.Null)
                throw new InvalidOperationException($"{nameof(subscriptionType)} = {subscriptionType}.");

            this.action = action;
            InputAction = inputAction;
            SubscriptionType = subscriptionType;
        }

        public abstract void Dispose();
    }
}
