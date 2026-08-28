using Unity.Entities;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterRotation : IComponentData
    {
        public static CharacterRotation Default => new() { Value = quaternion.identity };

        public quaternion Value;
    }
}
