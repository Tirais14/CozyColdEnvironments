using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [BurstCompile]
    public partial struct CharacterPhysicsJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        public readonly void Execute(
            in CharacterGravity gravity,
            in CharacterState state,
            in CharacterGroundCastResult groundCastResult,
            in CharacterRotation rotation,
            ref LocalTransform transform,
            ref CharacterVelocity characterVelocity,
            ref PhysicsVelocity velocity
            )
        {
            if (groundCastResult.Normal.TryGetValue(out float3 surfaceNormal))
            {
                float3 normalizedNormal = math.normalizesafe(surfaceNormal);

                float3 normalComponent = math.dot(characterVelocity.Linear, normalizedNormal) * normalizedNormal;
                characterVelocity.Linear -= normalComponent;
            }

            if (!state.IsGrounded)
                characterVelocity.Linear += new float3(0f, gravity.Value * DeltaTime, 0f);
            //else
            //    characterVelocity.Linear.y = 0f;

            velocity.Linear = characterVelocity.Linear;
            velocity.Angular = float3.zero;
            transform.Rotation = rotation.Value;
        }
    }
}
