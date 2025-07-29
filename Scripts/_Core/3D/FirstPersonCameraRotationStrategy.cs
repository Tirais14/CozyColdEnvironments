#nullable enable
using UnityEngine;
using UTIRLib.InputSystem;

namespace UTIRLib
{
    public class FirstPersonCameraRotationStrategy : IRotationStrategy
    {
        private readonly Transform transform;
        private readonly IInputAction<Vector2> inputAction;

        public float RotationSpeed { get; private set; }

        public FirstPersonCameraRotationStrategy(Transform cameraTransform,
                                                 IInputAction<Vector2> inputAction,
                                                 float rotationSpeed)
        {
            transform = cameraTransform;
            this.inputAction = inputAction;
            RotationSpeed = rotationSpeed;
        }

        /// <exception cref="System.ArgumentException"></exception>
        public void Rotate(float deltaTime)
        {
            if (deltaTime <= 0f)
                throw new System.ArgumentException(deltaTime.ToString(),
                                                   nameof(deltaTime));

            Vector3 angles = deltaTime * RotationSpeed * inputAction.Value;
            Quaternion targetRotation = Quaternion.Euler(0f, angles.y, 0f) * (transform.rotation * Quaternion.Euler(angles.x, 0f, 0f));

            transform.rotation = targetRotation;
        }

        /// <exception cref="System.ArgumentException"></exception>
        public void SetRotationSpeed(float newRotationSpeed)
        {
            if (newRotationSpeed <= 0f)
                throw new System.ArgumentException(newRotationSpeed.ToString(),
                                                   nameof(newRotationSpeed));

            RotationSpeed = newRotationSpeed;
        }
    }
}
