using Unity.Entities;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterGroundCastResult : IComponentData
    {
        public MaybeUnmanaged<float3> Normal;
    }
}
