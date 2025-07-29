#nullable enable

using System;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.InputSystem;
using UTIRLib.TwoD;
using UTIRLib.Vectors.Linq;

namespace UTIRLib.Controllers.FirstPerson
{
    public abstract class AFirstPersonMovementController : MonoX, IMovementController
    {
        [SerializeField]
        protected Camera firstPersonCamera;
        private readonly IInputAction<Vector2> moveInput;
        private float moveSpeed;

        public float MoveSpeed => moveSpeed;

        public AFirstPersonMovementController(Transform transform,
                                             Camera camera,
                                             IInputAction<Vector2> moveInput)
        {
            this.transform = transform;
            this.camera = camera;
            this.moveInput = moveInput;
        }

        /// <exception cref="ArgumentException"></exception>
        public void SetMoveSpeed(float newMoveSpeed)
        {
            if (newMoveSpeed <= 0f)
                throw new ArgumentException(newMoveSpeed.ToString(), nameof(newMoveSpeed));

            moveSpeed = newMoveSpeed;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void BindInput(IInputAction<Vector2> moveInput)
        {
            if (moveInput.IsNull())
                throw new ArgumentNullException(nameof(moveInput));
        }

        public void MoveByInput()
        {
            Direction2D direction = moveInput.Value.ToDirection2D();

            Vector3 forwardDirection = GetComponent<Camera>().transform.forward.Q()
                                                               .SetY(0)
                                                               .Normalize();

            float calculatedSpeed = moveSpeed * deltaTime;
            var rotation = direction switch
            {
                Direction2D.Down => Quaternion.Euler(0f, 180f, 0f),
                Direction2D.Left => Quaternion.Euler(0f, -90f, 0f),
                Direction2D.Right => Quaternion.Euler(0f, 90f, 0f),
                Direction2D.Up => Quaternion.Euler(0f, 0f, 0f),
                Direction2D.LeftDown => Quaternion.Euler(0f, -135f, 0f),
                Direction2D.LeftUp => Quaternion.Euler(0f, -45f, 0f),
                Direction2D.RightDown => Quaternion.Euler(0f, 135f, 0f),
                Direction2D.RightUp => Quaternion.Euler(0f, 45f, 0f),
                _ => throw new InvalidOperationException(direction.ToString()),
            };

            Move(rotation * forwardDirection, calculatedSpeed);
        }

        public void Move(Vector3 direction, float speed)
        {
            if (direction == Vector3.zero)
                return;
            if (speed <= 0)
                throw new System.ArgumentException(nameof(speed));

            transform.position += direction * speed;
        }
    }
}
