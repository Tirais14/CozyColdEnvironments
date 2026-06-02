using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    [BurstCompile]
    public struct BlobArrayJagged<T>
        where T : struct
    {
        public BlobArray<T> Values;
        public BlobArray<int> Lengths;
        public BlobArray<int> Offsets;

        public ref T this[int arrayIndex, int valueIndex] {
            get
            {
                var offset = Offsets[arrayIndex];
                return ref Values[offset + valueIndex];
            }
        }

        public int GetValuesIndex(int arrayIndex, int valueIndex)
        {
            if (arrayIndex >= Offsets.Length)
                return -1;

            int offset = Offsets[arrayIndex];
            return offset + valueIndex;
        }

        public bool IsInRange(int arrayIndex, int valueIndex)
        {
            int valuesIndex = GetValuesIndex(arrayIndex, valueIndex);
            return valuesIndex >= 0;
        }
        
        public bool IsInRangeIntEnum<TEnum>(TEnum arrayIndex, int valueIndex)
            where TEnum : unmanaged, Enum
        {
            return IsInRange(UnsafeUtility.As<TEnum, int>(ref arrayIndex), valueIndex);
        }

        public bool TryGetValue(int arrayIndex, int valueIndex, out T value)
        {
            if (!IsInRange(arrayIndex, valueIndex))
            {
                value = default;
                return false;
            }

            value = Values[arrayIndex];
            return true;
        }

        public bool TryGetValueIntEnum<TEnum>(TEnum arrayIndex, int valueIndex, out T value)
            where TEnum : unmanaged, Enum
        {
            return TryGetValue(Unsafe.As<TEnum, int>(ref arrayIndex), valueIndex, out value);
        }
    }

    public static class BlobArrayJaggedExtensions
    {
        public static int IndexOf<T>(this ref BlobArrayJagged<T> source, T value)
            where T : struct, IEquatable<T>
        {
            for (int i = 0; i < source.Values.Length; i++)
                if (source.Values[i].Equals(value))
                    return i;

            return -1;
        }

        public static bool Contains<T>(this ref BlobArrayJagged<T> source, T value)
            where T : struct, IEquatable<T>
        {
            return source.IndexOf(value) >= 0;
        }
    }
}
