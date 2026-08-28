using Unity.Entities;
using UnityEngine;

namespace CCEnvs.UnityX.ECS.Characters
{
    [UpdateAfter(typeof(CharacterPhysicsSystem))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct CharacterDebugSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (velocity, charState) in SystemAPI.Query<RefRO<CharacterVelocity>, RefRO<CharacterState>>())
            {
                this.PrintLog(velocity.ValueRO.Linear);
                this.PrintLog(charState.ValueRO.IsGrounded);
            }
        }
    }
}
