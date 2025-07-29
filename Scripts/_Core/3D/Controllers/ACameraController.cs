#nullable enable
using UnityEngine;
using UTIRLib.Attributes;

namespace UTIRLib.Controllers
{
    [RequireComponent(typeof(Camera))]
    public abstract class ACameraController : MonoX, ICameraController
    {
        [GetBySelf]
        public Camera Camera { get; private set; } = null!;

        [field: RequiredField]
        public IRotationController RotationController { get; protected set; } = null!;

        [field: SerializeField]
        public float RotationSpeed { get; protected set; } = 1f;
        
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
