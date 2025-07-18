using UnityEngine;
using UTIRLib.Attributes.Metadata;

#nullable enable
namespace UTIRLib.InputSystem
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
