using UnityEngine;
using CozyColdEnvironments.Attributes.Metadata;

#nullable enable
namespace CozyColdEnvironments.InputSystem
{
    public enum InputActionValueType : byte
    {
        None,

        [MetaType(typeof(bool))]
        Button,

        [MetaType(typeof(Vector2))]
        Vector2,

        [MetaType(typeof(Vector3))]
        Vector3,

        [MetaType(typeof(Quaternion))]
        Quternion
    }
}
