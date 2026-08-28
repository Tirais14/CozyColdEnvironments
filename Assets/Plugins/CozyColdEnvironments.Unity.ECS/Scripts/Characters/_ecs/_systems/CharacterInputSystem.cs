using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct CharacterInputSystem : ISystem
    {
        [BurstCompile]
        public readonly void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                .WithAll<CharacterInputs, CharacterMoveDirection>()
                .Build()
                );
        }

        [BurstCompile]
        public readonly void OnUpdate(ref SystemState state)
        {
            foreach (var (inputs, moveDirection) in SystemAPI.Query<RefRO<CharacterInputs>, RefRW<CharacterMoveDirection>>())
            {
                if (!inputs.ValueRO.MoveInput.Equals(float2.zero))
                {
                    float3 newMoveDirection = new float3(
                        x: inputs.ValueRO.MoveInput.x,
                        y: 0f,
                        z: inputs.ValueRO.MoveInput.y
                        );

                    moveDirection.ValueRW.Value = math.normalize(newMoveDirection);
                }
                else
                    moveDirection.ValueRW.Value = float3.zero;
            }
        }
    }
}
