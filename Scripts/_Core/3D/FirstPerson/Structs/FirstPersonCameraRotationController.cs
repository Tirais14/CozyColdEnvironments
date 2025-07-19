using UnityEngine;

#nullable enable
namespace UTIRLib.Controllers.FirstPerson
{
    public struct FirstPersonCameraRotationController : IRotationController
    {
        private readonly Transform transform;

        public bool inverseVertical;

        public FirstPersonCameraRotationController(Transform transform,
                                                   bool inverseVertical = false)
        {
            this.transform = transform;
            this.inverseVertical = inverseVertical;
        }

        /// <exception cref="System.ArgumentException"></exception>
        public readonly void Rotate(Vector3 delta, float speed)
        {
            if (delta == Vector3.zero)
                return;
            if (speed <= 0f)
                throw new System.ArgumentException(nameof(speed));

            delta = SwapAxis(delta);

            if (!inverseVertical)
                delta = Inverse(delta);

            delta = delta * speed;
            Quaternion target = Quaternion.Euler(0f, delta.y, 0f) * (transform.rotation * Quaternion.Euler(delta.x, 0f, 0f));

            transform.rotation = target;
        }

        private readonly Vector2 SwapAxis(Vector2 delta)
        {
            return new Vector2(delta.y, delta.x);
        }

        private readonly Vector2 Inverse(Vector2 delta)
        {
            delta.x *= -1;

            return delta;
        }
    }
}
