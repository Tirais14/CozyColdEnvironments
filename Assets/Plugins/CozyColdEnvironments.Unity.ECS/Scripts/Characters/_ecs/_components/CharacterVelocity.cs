using Unity.Entities;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterVelocity : IComponentData
    {
        public float3 Linear;
    }
}
