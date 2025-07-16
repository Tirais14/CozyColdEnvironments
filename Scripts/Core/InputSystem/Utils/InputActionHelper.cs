using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public static class InputActionHelper
    {
        public static Type GetValueType(InputAction inputAction) => inputAction.expectedControlType switch {
            "Button" => typeof(bool),
            "Vector2" => typeof(Vector2),
            "Vector3" => typeof(Vector3),
            "Quaternion" => typeof(Quaternion),
            _ => typeof(bool),
        };
    }
}