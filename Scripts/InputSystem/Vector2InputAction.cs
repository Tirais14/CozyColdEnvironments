using UnityEngine;
using UnityEngine.InputSystem;
using CozyColdEnvironments.Unity.Extensions;

#nullable enable
namespace CozyColdEnvironments.InputSystem
{
    public class Vector2InputAction : InputActionX<Vector2>
    {
        public Direction2D DirectionValue => Value.ToDirection2D();

        public Vector2InputAction(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
