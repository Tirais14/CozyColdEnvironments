#nullable enable
using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS
{
    public static class EntityExtensions
    {
        [BurstCompile]
        public static int GetHashCodeCustom(this Entity source)
        {
            return (int)math.hash(new int2(source.Index, source.Version));
        }

        [BurstDiscard]
        public static int GetHashCodeCustomWithOffset(this Entity source, string? hashOffset)
        {
            return (int)math.hash(new int3(source.Index, source.Version, hashOffset?.GetHashCode() ?? 0));
        }

        [BurstCompile]
        public static int GetHashCodeCustomWithIntEnumOffset<TEnum>(this Entity source, TEnum hashOffset)
            where TEnum : unmanaged, Enum
        {
            return (int)math.hash(new int3(source.Index, source.Version, UnsafeUtility.As<TEnum, int>(ref hashOffset)));
        }
    }
}
