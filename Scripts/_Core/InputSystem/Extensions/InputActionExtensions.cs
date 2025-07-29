using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace UTIRLib.InputSystem
{
    public static class InputActionExtensions
    {
        public static Type GetInputValueType(this InputAction value)
        {
            if (value.type == InputActionType.Button)
                return typeof(bool);

            return value.expectedControlType switch
            {
                "Vector2" => typeof(Vector2),
                "Vector3" => typeof(Vector3),
                "Quaternion" => typeof(Quaternion),
                _ => throw new NotImplementedException(value.expectedControlType),
            };
        }
    }
}
