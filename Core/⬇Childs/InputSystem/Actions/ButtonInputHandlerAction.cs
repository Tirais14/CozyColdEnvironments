using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public class ButtonInputHandlerAction : InputHandlerAction<bool>
    {
        public ButtonInputHandlerAction(InputAction inputAction) : base(inputAction)
        { }

        protected override bool ReadValue(InputAction.CallbackContext context) => context.ReadValueAsButton();
    }
}