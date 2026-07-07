using UnityEngine.InputSystem;
using UnityEngine.Scripting;

#nullable enable
namespace CCEnvs.UnityX.InputSystem.Rx
{
    public class ButtonActionRx : InputActionRx<bool>
    {
        [Preserve]
        public ButtonActionRx(InputAction inputAction) : base(inputAction)
        {
        }

        protected override bool ReadValue(InputAction.CallbackContext context)
        {
            return context.ReadValueAsButton();
        }
    }
}
