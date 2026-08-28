using Unity.Burst;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [BurstCompile]
    public struct CharacterIdleState
    {
        [BurstCompile]
        public readonly void OnEnter(
            ref CharacterVelocity velocity
            )
        {
            velocity.Linear = new float3(0f, velocity.Linear.y, 0f);
        }
    }
}
