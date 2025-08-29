using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.InputSystem
{
    public static class InputActionExtensions
    {
        public static Type? GetInputValueType(this InputAction value)
        {
            if (value.type == InputActionType.Button)
                return typeof(bool);

            return value.expectedControlType switch
            {
                "Axis" => typeof(float),
                "Double" => typeof(double),
                "Vector2" => typeof(Vector2),
                "Vector3" => typeof(Vector3),
                "Integer" => typeof(int),
                "Quaternion" => typeof(Quaternion),
                _ => null,
            };
        }

        public static bool TryGetInputValueType(this InputAction value,
                                                [NotNullWhen(true)] out Type? result)
        {
            result = value.GetInputValueType();

            return result is not null;
        }
    }
}
