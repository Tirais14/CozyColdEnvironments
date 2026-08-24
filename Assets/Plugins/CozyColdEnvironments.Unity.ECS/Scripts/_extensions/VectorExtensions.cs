using Unity.Mathematics;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class VectorExtensions
    {
        public static float2 ToFloat2(this in Vector2 source)
        {
            return new float2(source.x, source.y);
        }

        public static float3 ToFloat3(this in Vector3 source)
        {
            return new float3(source.x, source.y, source.z);
        }

        public static int2 ToInt2(this in Vector2Int source)
        {
            return new int2(source.x, source.y);
        }

        public static int3 ToInt3(this in Vector3Int source)
        {
            return new int3(source.x, source.y, source.z);
        }
    }
}
