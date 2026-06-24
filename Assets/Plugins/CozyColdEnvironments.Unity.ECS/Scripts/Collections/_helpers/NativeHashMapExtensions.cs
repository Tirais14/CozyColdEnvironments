using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    public static class NativeHashMapExtensions
    {
        [BurstCompile]
        public static int GetCount<TKey, TValue>(this in NativeHashMap<TKey, TValue> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            if (!source.IsCreated)
                return 0;

            return source.Count;
        }

        [BurstCompile]
        public static int GetCount<TKey, TValue>(this in NativeHashMap<TKey, TValue>.ReadOnly source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            if (!source.IsCreated)
                return 0;

            return source.Count;
        }
    }
}
