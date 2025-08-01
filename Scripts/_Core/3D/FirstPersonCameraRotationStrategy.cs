#nullable enable
using UnityEngine;
using UTIRLib.InputSystem;

namespace UTIRLib
{
    public class FirstPersonCameraRotationStrategy : IRotationStrategy
    {
        private readonly Transform transform;
        private readonly IInputAction<Vector2> inputAction;

        private float verticalAngle = 0f;
        private float horizontalAngle = 0f;

        public float RotationSpeed { get; private set; }

        public FirstPersonCameraRotationStrategy(Camera camera,
                                                 IInputAction<Vector2> inputAction,
                                                 float rotationSpeed)
        {
            transform = camera.transform;
            this.inputAction = inputAction;
            RotationSpeed = rotationSpeed;

            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <exception cref="System.ArgumentException"></exception>
        public void Rotate(float deltaTime)
        {
            if (deltaTime <= 0f)
                throw new System.ArgumentException(deltaTime.ToString(),
                                                   nameof(deltaTime));

            float inputX = inputAction.Value.x * deltaTime * RotationSpeed;
            float inputY = inputAction.Value.y * deltaTime * RotationSpeed;

            verticalAngle -= inputY;
            verticalAngle = Mathf.Clamp(verticalAngle, -90f, 90f);

            horizontalAngle += inputX;

            transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

            ////Swap axis
            //angles.Set(angles.y * -1, angles.x, angles.z);

            //Quaternion targetRotation = Quaternion.Euler(0f, angles.y, 0f) * (transform.rotation * Quaternion.Euler(angles.x, 0f, 0f));

            //transform.rotation = targetRotation;
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
