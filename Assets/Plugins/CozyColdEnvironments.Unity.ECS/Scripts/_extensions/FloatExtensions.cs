using Unity.Mathematics;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class FloatExtensions
    {
        public static int2 FloorToInt(this in float2 source)
        {
            return (int2)math.floor(source);
        }

        public static int3 FloorToInt(this in float3 source)
        {
            return (int3)math.floor(source);
        }

        public static float2 ToFloat2(this in float3 source)
        {
            return new float2
            {
                x = source.x,
                y = source.y
            };
        }

        public static float3 ToFloat3(this in float2 source)
        {
            return new float3
            {
                x = source.x,
                y = source.y
            };
        }

        public static Vector2 AsVector2(this in float2 source)
        {
            return new Vector2(source.x, source.y);
        }

        public static Vector3 AsVector3(this in float3 source)
        {
            return new Vector3(source.x, source.y, source.z);
        }

        public static Vector4 AsVector4(this in float4 source)
        {
            return new Vector4(source.x, source.y, source.z, source.w);
        }

        public static Vector2 ToVector2(this in float3 source)
        {
            return new Vector2(source.x, source.y);
        }
    }
}
