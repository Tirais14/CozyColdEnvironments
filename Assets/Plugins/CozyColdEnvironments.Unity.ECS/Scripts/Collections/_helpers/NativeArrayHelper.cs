using Unity.Burst;
using Unity.Collections;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    public static class NativeArrayHelper
    {
        [BurstCompile]
        public static int GetLength<T>(this in NativeArray<T> source)
            where T : struct
        {
            if (!source.IsCreated)
                return 0;

            return source.Length;
        }

        [BurstCompile]
        public static int GetLength<T>(this in NativeArray<T>.ReadOnly source)
            where T : struct
        {
            if (!source.IsCreated)
                return 0;

            return source.Length;
        }
    }
}
