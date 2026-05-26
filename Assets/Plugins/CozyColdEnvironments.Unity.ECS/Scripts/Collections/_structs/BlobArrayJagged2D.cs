using System;
using Unity.Burst;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    [BurstCompile]
    public struct BlobArrayJagged2D<T>
        where T : struct
    {
        public BlobArray<T> Values;
        public BlobArray<int> Lengths;
        public BlobArray<int> Offsets;

        public ref T this[int index1, int index2] {
            get
            {
                var offset = Offsets[index1];
                return ref Values[offset + index2];
            }
        }

        public int GetValuesIndex(int index1, int index2)
        {
            if (index1 >= Offsets.Length)
                return -1;

            int offset = Offsets[index1];
            return offset + index2;
        }

        public bool IsInRange(int index1, int index2)
        {
            int valuesIndex = GetValuesIndex(index1, index2);
            return valuesIndex >= 0;
        }
    }

    public static class BlobArrayJagged2DExtensions
    {
        public static int IndexOf<T>(this ref BlobArrayJagged2D<T> source, T value)
            where T : struct, IEquatable<T>
        {
            for (int i = 0; i < source.Values.Length; i++)
                if (source.Values[i].Equals(value))
                    return i;

            return -1;
        }

        public static bool Contains<T>(this ref BlobArrayJagged2D<T> source, T value)
            where T : struct, IEquatable<T>
        {
            return source.IndexOf(value) >= 0;
        }
    }
}
