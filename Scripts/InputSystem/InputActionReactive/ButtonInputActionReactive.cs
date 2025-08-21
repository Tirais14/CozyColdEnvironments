using UnityEngine.InputSystem;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
{
    public class ButtonInputActionReactive : InputActionReactive<bool>
    {
        public ButtonInputActionReactive(InputAction inputAction) : base(inputAction)
        {
        }

        protected override bool GetInputValue(InputAction.CallbackContext context)
        {
            return context.ReadValueAsButton();
        }
    }
}
