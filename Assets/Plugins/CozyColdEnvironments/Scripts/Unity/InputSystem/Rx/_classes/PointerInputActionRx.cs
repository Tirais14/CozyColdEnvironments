using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class PointerInputActionRx : InputActionRx<Vector2>
    {
        /// <summary>
        /// Uses <see cref="Camera.main"/> to resolve
        /// </summary>
        public Vector2 WorldPoint => Camera.main.ScreenToWorldPoint(InputValue);

        public PointerInputActionRx(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
