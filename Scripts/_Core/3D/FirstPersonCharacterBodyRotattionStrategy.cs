using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public sealed class FirstPersonCharacterBodyRotattionStrategy : IRotationStrategy
    {
        private readonly Transform body;
        private readonly FirstPersonCameraRotationStrategy cameraRotationStrategy;
        private readonly Transform cameraTransform;

        public float RotationSpeed => cameraRotationStrategy.RotationSpeed;

        public FirstPersonCharacterBodyRotattionStrategy(Transform body,
            FirstPersonCameraRotationStrategy cameraRotationStrategy,
            Camera characterCamera)
        {
            this.body = body;
            this.cameraRotationStrategy = cameraRotationStrategy;
            cameraTransform = characterCamera.transform;
        }

        public void Rotate(float deltaTime)
        {
            body.rotation = cameraTransform.rotation;
        }

        void IRotationStrategy.SetRotationSpeed(float newRotationSpeed)
        {
        }
    }
}
