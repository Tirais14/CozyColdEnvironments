using UnityEngine.InputSystem;

#nullable enable
namespace UTIRLib.InputSystem
{
    public class ButtonInputAction : InputActionX<bool>
    {
        public ButtonInputAction(InputAction inputAction) : base(inputAction)
        {
        }

        protected override bool ReadValue(InputAction.CallbackContext context)
        {
            return context.ReadValueAsButton();
        }
    }
}
