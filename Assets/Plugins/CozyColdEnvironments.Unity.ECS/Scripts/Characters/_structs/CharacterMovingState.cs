#nullable enable
using Unity.Burst;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS.Characters
{
    [BurstCompile]
    public struct CharacterMovingState
    {
        [BurstCompile]
        public readonly void Execute(
            CharacterMoveSpeed speed,
            in CharacterMoveDirection direction,
            ref CharacterVelocity velocity
            )
        {
            float3 tempVelocity = speed.Value * direction.Value;
            velocity.Linear = new float3(tempVelocity.x, velocity.Linear.y, tempVelocity.z);
        }
    }
}
