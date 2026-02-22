using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public class PointerInputActionRx : InputActionRx<Vector2>
    {
        /// <summary>
        /// Uses <see cref="Camera.main"/> to resolve
        /// </summary>
        public Vector2 WorldPoint => Camera.main.ScreenToWorldPoint(InputValue);

        [Preserve]
        public PointerInputActionRx(InputAction inputAction) : base(inputAction)
        {
        }
    }
}
