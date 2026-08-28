using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace CCEnvs.UnityX.ECS.Characters
{
    [BurstCompile]
    [UpdateAfter(typeof(CharacterStateMachineSystem))]
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct CharacterPhysicsSystem : ISystem
    {
        public readonly void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                .WithAll<CharacterVelocity, PhysicsVelocity, LocalTransform>()
                .Build()
                );
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new CharacterPhysicsJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }
            .ScheduleParallel(state.Dependency);
        }
    }
}
