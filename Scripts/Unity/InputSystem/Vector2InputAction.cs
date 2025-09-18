using UnityEngine;
using UnityEngine.InputSystem;
using CCEnvs.Unity.Extensions;

#nullable enable
namespace CCEnvs.Unity.InputSystem
{
    public class Vector2InputAction : InputActionX<Vector2>
    {
        public Direction2D DirectionValue => Value.ToDirection2D();

        public Vector2InputAction(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
