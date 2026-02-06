using UnityEngine;

namespace CCEnvs.Unity._2D
{
    public static class ObjectMove
    {
        public static void ByPhysics(Rigidbody2D body,
                                     Vector2 dir,
                                     float speed,
                                     float deltaTime)
        {
            const float SPEED_MULTIPLIER = 50; //Need for smaller input speed values

            CC.Guard.IsNotNull(body, nameof(body));

            if (dir == Vector2.zero || speed <= 0f || deltaTime <= 0f)
            {
                body.linearVelocity = Vector2.zero;
                return;
            }

            body.linearVelocity = deltaTime * speed * SPEED_MULTIPLIER * dir.normalized;
        }
    }
}
