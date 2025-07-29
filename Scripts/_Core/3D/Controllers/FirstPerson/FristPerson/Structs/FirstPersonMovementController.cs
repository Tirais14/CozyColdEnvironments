#nullable enable

using System;
using UnityEngine;
using UTIRLib.InputSystem;
using UTIRLib.TwoD;
using UTIRLib.Vectors.Linq;

namespace UTIRLib.Controllers.FirstPerson
{
    public class FirstPersonMovementController : MonoX, IMovementController
    {
        public IInputAction<Vector2>? MoveInput { get; set; }

        [SerializeField]
        protected Camera firstPersonCamera;

        [Min(1E-06f)]
        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        /// <exception cref="ArgumentException"></exception>
        public void SetMoveSpeed(float newMoveSpeed)
        {
            if (newMoveSpeed <= 0f)
                throw new ArgumentException(newMoveSpeed.ToString(), nameof(newMoveSpeed));

            MoveSpeed = newMoveSpeed;
        }

        public void Move(Vector3 direction, float speed)
        {
            if (direction == Vector3.zero)
                return;
            if (speed <= 0)
                throw new ArgumentException(nameof(speed));

            transform.position += direction * speed;
        }

        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void MoveInternal()
        {
            Direction2D direction = MoveInput!.Value.ToDirection2D();

            Vector3 forwardDirection = firstPersonCamera.transform.forward.Q()
                                                                          .SetY(0)
                                                                          .Normalize();

            float calculatedSpeed = MoveSpeed * Time.fixedDeltaTime;
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

        private void FixedUpdate()
        {
            if (MoveInput is not null && MoveInput.Value != Vector2.zero)
                MoveInternal();
        }
    }
}
