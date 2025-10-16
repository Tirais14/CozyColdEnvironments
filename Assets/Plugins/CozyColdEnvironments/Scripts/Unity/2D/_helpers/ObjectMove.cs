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
            CC.Guard.NullArgument(body, nameof(body));

            if (dir == Vector2.zero || speed <= 0f || deltaTime <= 0f)
                return;

            body.linearVelocity = deltaTime * speed * dir;
        }
    }
}
