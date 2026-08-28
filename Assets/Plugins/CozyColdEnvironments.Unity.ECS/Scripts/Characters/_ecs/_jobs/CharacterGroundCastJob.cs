using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public partial struct CharacterGroundCastJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton Physics;

        public readonly void Execute(
            in CharacterGroundCastInfo groundCastInfo,
            in CharacterColliderInfo colliderInfo,
            in LocalTransform transform,
            ref CharacterState state,
            ref CharacterGroundCastResult result
            )
        {
            float3 castPoint = transform.Position + groundCastInfo.CastPoint;
            float maxDistance = colliderInfo.Radius + groundCastInfo.CastDistance;

            var hits = new NativeList<DistanceHit>(Allocator.Temp);

            bool anyHit = Physics.OverlapSphere(
                castPoint,
                maxDistance,
                ref hits,
                groundCastInfo.Filter,
                QueryInteraction.IgnoreTriggers
            );

            bool isGrounded = false;
            float3 groundNormal = default;
            float closestDistance = float.MaxValue;

            if (anyHit)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    DistanceHit hit = hits[i];

                    // Поверхность под ногами: нормаль смотрит вверх
                    // hit.Position - hit.Distance * hit.SurfaceNormal даёт точку на поверхности
                    // Но проще: проверяем, что позиция хита ниже castPoint
                    bool isBelow = hit.Position.y < castPoint.y;
                    bool isUpwardFacing = hit.SurfaceNormal.y > 0.1f;
                    bool isClose = hit.Distance <= maxDistance;

                    if (isBelow && isUpwardFacing && isClose && hit.Distance < closestDistance)
                    {
                        closestDistance = hit.Distance;
                        groundNormal = hit.SurfaceNormal;
                        isGrounded = true;
                    }
                }
            }

            state.IsGrounded = isGrounded;
            result.Normal = isGrounded ? groundNormal : default;

            hits.Dispose();
        }
    }
}
