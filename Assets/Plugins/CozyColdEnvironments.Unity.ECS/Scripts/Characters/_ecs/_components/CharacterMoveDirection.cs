using Unity.Entities;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterMoveDirection : IComponentData
    {
        public float3 Value;
    }
}
