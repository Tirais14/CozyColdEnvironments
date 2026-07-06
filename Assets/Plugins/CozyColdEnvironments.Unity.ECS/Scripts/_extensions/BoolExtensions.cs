using Unity.Burst;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class BoolExtensions
    {
        [BurstCompile]
        public static bool All(this in bool2 source)
        {
            return source.x && source.y;
        }
        [BurstCompile]
        public static bool All(this in bool3 source)
        {
            return source.x && source.y && source.z;
        }
        [BurstCompile]
        public static bool All(this in bool4 source)
        {
            return source.x && source.y && source.z && source.w;
        }

        [BurstCompile]
        public static bool Any(this in bool2 source)
        {
            return source.x || source.y;
        }
        [BurstCompile]
        public static bool Any(this in bool3 source)
        {
            return source.x || source.y || source.z;
        }
        [BurstCompile]
        public static bool Any(this in bool4 source)
        {
            return source.x || source.y || source.z || source.w;
        }
    }
}
