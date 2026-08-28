using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterGroundCastInfo : ISharedComponentData
    {
        public float3 CastPoint;

        public float CastDistance;

        public CollisionFilter Filter;
    }
}
