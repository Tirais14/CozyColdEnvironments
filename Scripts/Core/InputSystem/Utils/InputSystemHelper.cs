using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public static class InputSystemHelper
    {
        public static InputActionValueType ResolveInputActionValueType(InputAction inputAction)
        {
            if (inputAction.expectedControlType.Contains(
                nameof(Vector2), StringComparison.InvariantCulture))
            {
                return InputActionValueType.Vector2;
            }
            else if (inputAction.expectedControlType.Contains(
                nameof(Vector3), StringComparison.InvariantCulture))
            {
                return InputActionValueType.Vector3;
            }
            else if (inputAction.expectedControlType.Contains(
                nameof(Quaternion), StringComparison.InvariantCulture))
            {
                return InputActionValueType.Quaternion;
            }
            else if (inputAction.expectedControlType.Equals("button", StringComparison.InvariantCultureIgnoreCase) ||
                inputAction.activeValueType == typeof(bool) ||
                inputAction.type == InputActionType.Button)
            {
                return InputActionValueType.Button;
            }
            else
            {
                return InputActionValueType.None;
            }
        }
    }
}