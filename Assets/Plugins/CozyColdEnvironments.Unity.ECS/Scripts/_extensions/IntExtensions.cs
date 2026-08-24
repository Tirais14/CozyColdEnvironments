using Unity.Mathematics;
using UnityEngine;

namespace CCEnvs.UnityX.ECS
{
    public static class IntExtensions
    {
        public static int2 WithX(this int2 source, int x)
        {
            source.x = x;
            return source;
        }

        public static int2 WithY(this int2 source, int y)
        {
            source.y = y;
            return source;
        }

        public static int3 WithX(this int3 source, int x)
        {
            source.x = x;
            return source;
        }

        public static int3 WithY(this int3 source, int y)
        {
            source.y = y;
            return source;
        }

        public static int3 WithZ(this int3 source, int z)
        {
            source.z = z;
            return source;
        }

        public static int4 WithX(this int4 source, int x)
        {
            source.x = x;
            return source;
        }

        public static int4 WithY(this int4 source, int y)
        {
            source.y = y;
            return source;
        }

        public static int4 WithZ(this int4 source, int z)
        {
            source.z = z;
            return source;
        }

        public static int4 WithW(this int4 source, int w)
        {
            source.w = w;
            return source;
        }
    }
}
