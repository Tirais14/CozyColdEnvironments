#nullable enable
using UnityEngine;
using UTIRLib.InputSystem;

#pragma warning disable IDE0044
namespace UTIRLib.Controllers.FirstPerson
{
    public class FirstPersonCameraController : ACameraController
    {
        public IInputAction<Vector2>? MoveInput { get; set; }

        protected override void OnAwake()
        {
            base.OnAwake();

            RotationController = new FirstPersonCameraRotationController();
        }

        private void FixedUpdate()
        {
            if (MoveInput is not null
                &&
                MoveInput.Value != Vector2.zero
                )
                RotationController.Rotate(MoveInput.Value,
                                          RotationSpeed * Time.fixedDeltaTime);
        }
    }
}
