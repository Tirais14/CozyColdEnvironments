using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.InputSystem
{
    public class InputActionNotFoundException : TirLibException
    {
        public InputActionNotFoundException()
        {
        }

        public InputActionNotFoundException(string inputActionName)
            : base($"Input action {inputActionName} not found.")
        {
        }
    }
}