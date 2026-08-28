using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(CharacterPhysicsSystem))]
    public partial struct CharacterStateMachineSystem : ISystem
    {
        [BurstCompile]
        public readonly void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<CharacterState, CharacterStates>()
                    .Build()
                );
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new CharacterGroundCastJob
            {
                Physics = SystemAPI.GetSingleton<PhysicsWorldSingleton>()
            }
            .ScheduleParallel(state.Dependency);

            state.Dependency = new CharacterStateMachineJob()
                .ScheduleParallel(state.Dependency);
        }
    }
}
