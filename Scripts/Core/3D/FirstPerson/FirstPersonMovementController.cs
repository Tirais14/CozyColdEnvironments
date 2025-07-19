#nullable enable

using UnityEngine;

namespace UTIRLib.Controllers.FirstPerson
{
    public readonly struct FirstPersonMovementController : IMovementController
    {
        private readonly Transform transform;

        public FirstPersonMovementController(Transform transform)
        {
            this.transform = transform;
        }

        public readonly void Move(Vector3 inputValue, float speed)
        {
            if (inputValue == Vector3.zero)
                return;
            if (speed <= 0)
                throw new System.ArgumentException(nameof(speed));

            Vector3 direction = transform.rotation * inputValue;

            transform.position += direction * speed;
        }
    }
}
