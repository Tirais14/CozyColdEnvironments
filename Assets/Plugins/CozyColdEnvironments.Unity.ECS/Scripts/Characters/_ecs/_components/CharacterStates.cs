using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterStates : ISharedComponentData
    {
        public static CharacterStates Default { get; }

        public CharacterIdleState Idle;

        public CharacterMovingState Moving;
    }
}
