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
        public readonly void Move(Vector3 direction, float speed)
        {
            if (direction == Vector3.zero)
                return;
            if (speed <= 0)
                throw new System.ArgumentException(nameof(speed));

            transform.position += direction * speed;
        }
    }
}
