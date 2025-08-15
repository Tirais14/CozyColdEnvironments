using System;
using UnityEngine.InputSystem;
using UTIRLib.Diagnostics;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem
{
    public class InputActionPredicated : InputActionX
    {
        private readonly Func<bool> predicate;

        public InputActionPredicated(InputAction inputAction,
                                     Func<bool> predicate)
            : 
            base(inputAction)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }

        public InputActionPredicated(InputAction inputAction,
                                     params IInputAction<bool>[] modifiers)
            :
            base(inputAction)
        {
            if (modifiers.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(modifiers), modifiers);

            predicate = () =>
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    if (!modifiers[i].Value)
                        return false;
                }

                return true;
            };
        }

        protected override void StartedEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.StartedEvent(context);
        }

        protected override void PerformedEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.PerformedEvent(context);
        }

        protected override void CanceledEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.CanceledEvent(context);
        }
    }

    public class InputActionPredicated<T> : InputActionX<T>
        where T : struct
    {
        private readonly Func<bool> predicate;


        public InputActionPredicated(InputAction inputAction,
                                     Func<bool> predicate)
            :
            base(inputAction)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }

        public InputActionPredicated(InputAction inputAction,
                                     params IInputAction<bool>[] modifiers)
            :
            base(inputAction)
        {
            if (modifiers.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(modifiers), modifiers);

            predicate = () =>
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    if (!modifiers[i].Value)
                        return false;
                }

                return true;
            };
        }

        protected override void StartedEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.StartedEvent(context);
        }

        protected override void PerformedEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.PerformedEvent(context);
        }

        protected override void CanceledEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.CanceledEvent(context);
        }

        protected override void ValueStartedEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.ValueStartedEvent(context);
        }

        protected override void ValuePerformedEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.ValuePerformedEvent(context);
        }

        protected override void ValueCanceledEvent(CallbackContext context)
        {
            if (!predicate())
                return;

            base.ValueCanceledEvent(context);
        }
    }
}
