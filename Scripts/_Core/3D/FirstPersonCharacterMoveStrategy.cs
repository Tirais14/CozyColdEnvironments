#nullable enable
using System;
using UnityEngine;
using UTIRLib.InputSystem;
using UTIRLib.TwoD;
using UTIRLib.Vectors.Linq;

namespace UTIRLib
{
    public class FirstPersonCharacterMoveStrategy : IMoveStrategy
    {
        private readonly Rigidbody rb;
        private readonly Camera fpCamera;
        private readonly IInputAction<Vector2> inputAction;

        public float MoveSpeed { get; private set; }

        public FirstPersonCharacterMoveStrategy(Rigidbody rigidbody,
                                                Camera fpCamera,
                                                IInputAction<Vector2> inputAction,
                                                float moveSpeed)
        {
            rb = rigidbody;
            this.fpCamera = fpCamera;
            this.inputAction = inputAction;
            MoveSpeed = moveSpeed;
        }

        /// <exception cref="ArgumentException"></exception>
        public void Move(float deltaTime)
        {
            if (deltaTime <= 0f)
                throw new ArgumentException(deltaTime.ToString(),
                                                   nameof(deltaTime));

            if (inputAction.Value == Vector2.zero)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                return;
            }

            Direction2D inputDirection = inputAction.Value.ToDirection2D();

            Vector3 cameraDirection = fpCamera.transform.forward.Q()
                                                                .SetY(0)
                                                                .Normalize();

            var vectorRotation = inputDirection switch
            {
                Direction2D.Down => Quaternion.Euler(0f, 180f, 0f),
                Direction2D.Left => Quaternion.Euler(0f, -90f, 0f),
                Direction2D.Right => Quaternion.Euler(0f, 90f, 0f),
                Direction2D.Up => Quaternion.Euler(0f, 0f, 0f),
                Direction2D.LeftDown => Quaternion.Euler(0f, -135f, 0f),
                Direction2D.LeftUp => Quaternion.Euler(0f, -45f, 0f),
                Direction2D.RightDown => Quaternion.Euler(0f, 135f, 0f),
                Direction2D.RightUp => Quaternion.Euler(0f, 45f, 0f),
                _ => throw new InvalidOperationException(inputDirection.ToString()),
            };

            Vector3 delta = (deltaTime * MoveSpeed * (vectorRotation * cameraDirection))
                .Q()
                .SetY(rb.linearVelocity.y);

            rb.linearVelocity = delta;
        }

        /// <exception cref="System.ArgumentException"></exception>
        public void SetMoveSpeed(float newMoveSpeed)
        {
            if (newMoveSpeed <= 0f)
                throw new ArgumentException(newMoveSpeed.ToString(),
                                                   nameof(newMoveSpeed));

            MoveSpeed = newMoveSpeed;
        }
    }
}
