using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Unity.Extensions;

#nullable enable
namespace UTIRLib.InputSystem
{
    public class Vector2InputAction : InputActionX<Vector2>
    {
        public Direction2D DirectionValue => Value.ToDirection2D();

        public Vector2InputAction(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
