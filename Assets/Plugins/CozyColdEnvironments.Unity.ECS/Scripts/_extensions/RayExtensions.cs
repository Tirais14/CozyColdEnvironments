using Unity.Physics;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class RayExtensions
    {
        public static RaycastInput ToRaycastInput(
            this UnityEngine.Ray source,
            float distance,
            CollisionFilter? filter = null
            )
        {
            return new RaycastInput
            {
                Start = source.origin,
                End = source.direction * distance,
                Filter = filter ?? new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u
                }
            };
        }
    }
}
