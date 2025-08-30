using CCEnvs.Unity.InputSystem;
using System;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.InputSystem
{
    public sealed class InputActionSubscription 
        : 
        AInputActionSubscription<IInputAction, Delegate>
    {
        public InputActionSubscription(IInputAction inputAction,
                                       Action<CallbackContext> action,
                                       InputActionSubscriptionType subscriptionType)
            :
            base(inputAction,
                 action,
                 subscriptionType)
        {
        }
        public InputActionSubscription(IInputAction inputAction,
                                       Action action,
                                       InputActionSubscriptionType subscriptionType) 
            : 
            base(inputAction,
                 action,
                 subscriptionType)
        {
        }

        public override void Dispose()
        {
            if (action is Action<CallbackContext> contexted)
            {
                InputAction.Unsubscribe(contexted, SubscriptionType);

                return;
            }

            InputAction.Unsubscribe((action as Action)!, SubscriptionType);
        }
    }
    public sealed class InputActionSubscription<T> 
        :
        AInputActionSubscription<IInputAction<T>, Action<T>>
        where T : struct
    {
        public InputActionSubscription(IInputAction<T> inputAction,
                                       Action<T> action,
                                       InputActionSubscriptionType subscriptionType) 
            :
            base(inputAction,
                 action,
                 subscriptionType)
        {
        }

        public override void Dispose()
        {
            InputAction.Unsubscribe(action, SubscriptionType);
        }
    }
}
