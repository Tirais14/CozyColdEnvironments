using System;
using Unity.Burst;
using Unity.Collections;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    public static class NativeListHelper
    {
        [BurstCompile]
        public static int GetLength<T>(this in NativeList<T> source)
            where T : unmanaged
        {
            if (!source.IsCreated)
                return 0;

            return source.Length;
        }

        [BurstCompile]
        public static bool StructuralEquals<T>(this in NativeList<T> left, in NativeList<T> right)
            where T : unmanaged, IEquatable<T>
        {
            if (left.IsCreated != right.IsCreated
                ||
                left.GetLength() != right.GetLength())
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
                if (!left[i].Equals(right[i]))
                    return false;

            return true;
        }

        [BurstCompile]
        public static int GetStructuralHashCode<T>(this in NativeList<T> source)
            where T : unmanaged, IEquatable<T>
        {
            var hash = new HashCode();

            for (int i = 0; i < source.Length; i++)
                hash.Add(source[i]);

            return hash.ToHashCode();
        }
    }
}
