using UnityEngine;

#nullable enable
#pragma warning disable S1244
namespace CCEnvs.Unity.ThreeD
{
    public static class RigidBodyHelper
    {
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void MoveByPhysics(Rigidbody rigidbody,
                                         Vector3 direction,
                                         float speed)
        {
            if (rigidbody == null)
                throw new System.ArgumentNullException(nameof(rigidbody));
            if (float.IsNegative(speed))
                throw new System.ArgumentException(nameof(speed));
            if (direction == Vector3.zero || speed == 0f)
            {
                rigidbody.linearVelocity = Vector3.zero;
                return;
            }

            rigidbody.linearVelocity = speed * Time.fixedDeltaTime * direction;
        }
    }
}
