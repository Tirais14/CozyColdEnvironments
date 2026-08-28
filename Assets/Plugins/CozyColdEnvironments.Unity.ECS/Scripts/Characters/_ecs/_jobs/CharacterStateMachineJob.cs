using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [BurstCompile]
    public partial struct CharacterStateMachineJob : IJobEntity
    {
        public readonly void Execute(
            CharacterMoveSpeed speed,
            in CharacterMoveDirection direction,
            in CharacterInputs inputs,
            in CharacterStates states,
            ref CharacterState state,
            ref CharacterVelocity velocity
            )
        {
            if (!inputs.MoveInput.Equals(float2.zero))
                state.Value = CharacterStateType.Moving;
            else
            {
                state.Value = CharacterStateType.Idle;
                states.Idle.OnEnter(ref velocity);
            }

            switch (state.Value)
            {
                case CharacterStateType.Idle:
                    break;
                case CharacterStateType.Moving:
                    states.Moving.Execute(speed, direction, ref velocity);
                    break;
                default:
                    break;
            }
        }
    }
}
