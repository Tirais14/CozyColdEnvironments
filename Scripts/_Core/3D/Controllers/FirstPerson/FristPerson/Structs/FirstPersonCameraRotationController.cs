using UnityEngine;

#nullable enable
#pragma warning disable S2234
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
        public readonly void Rotate(Vector3 angles, float speed)
        {
            if (angles == Vector3.zero)
                return;
            if (speed <= 0f)
                throw new System.ArgumentException(nameof(speed));

            angles = SwapAxis(angles);

            if (!inverseVertical)
                angles = InverseVertical(angles);

            angles *= speed;
            Quaternion target = Quaternion.Euler(0f,
                angles.y,
                0f) * (transform.rotation * Quaternion.Euler(angles.x, 0f, 0f));

            transform.rotation = target;
        }

        private static Vector2 SwapAxis(Vector2 delta)
        {
            return new Vector2(delta.y, delta.x);
        }

        private static Vector2 InverseVertical(Vector2 delta)
        {
            delta.x *= -1;

            return delta;
        }
    }
}
