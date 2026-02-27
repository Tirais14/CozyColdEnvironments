using CCEnvs.Attributes.Metadata;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.InputSystem
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
