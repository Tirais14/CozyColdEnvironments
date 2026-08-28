using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterState : IComponentData
    {
        public static CharacterState Default { get; } = new();

        public CharacterStateType Value;

        public bool IsGrounded;
    }
}
