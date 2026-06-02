#nullable enable
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS.Collections
{
    public ref struct BlobBuilderArrayJagged<T>
        where T : struct
    {
        public BlobBuilderArray<T> ValuesAB;
        public BlobBuilderArray<int> LengthsAB;
        public BlobBuilderArray<int> OffsetsAB;

        public int arrayPointer;
        private int valuesPointer;
        private int arrayLength;

        private List<T>? chunk;

        private bool arrayBegan;

        public void BeginArray(int capacity = 4)
        {
            if (arrayBegan)
                throw new InvalidOperationException("Cannot begin array more than one time");

            capacity = math.max(capacity, 1);

            if (chunk is null)
                chunk = new List<T>(capacity);
            else if (chunk.Capacity < capacity)
                chunk.Capacity = capacity;

            arrayPointer++;

            if (arrayPointer >= LengthsAB.Length
                ||
                arrayPointer >= OffsetsAB.Length)
            {
                throw new IndexOutOfRangeException(arrayPointer.ToString());
            }

            arrayBegan = true;
        }

        public void Add(T value)
        {
            if (!arrayBegan)
                throw new InvalidOperationException("Before add new element array must began");

            ValuesAB[valuesPointer++] = value;
            arrayLength++;
        }

        public void EndArray()
        {
            LengthsAB[arrayPointer] = arrayLength;
            OffsetsAB[arrayPointer] = valuesPointer - arrayLength;

            chunk?.Clear();
            valuesPointer = 0;
            arrayLength = 0;
            arrayBegan = false;
        }
    }

    public static class BlobBuilderExtensions
    {
        public static BlobBuilderArrayJagged<T> AllocateJagged<T>(
            this in BlobBuilder builder,
            ref BlobArrayJagged<T> array,
            int arrayCount,
            int valueCount
            )
            where T : struct
        {
            BlobBuilderArray<T> valuesAB = builder.Allocate(ref array.Values, valueCount);
            BlobBuilderArray<int> offsetsAB = builder.Allocate(ref array.Offsets, arrayCount);
            BlobBuilderArray<int> lengthsAB = builder.Allocate(ref array.Lengths, arrayCount);

            return new BlobBuilderArrayJagged<T>
            {
                ValuesAB = valuesAB,
                OffsetsAB = offsetsAB,
                LengthsAB = lengthsAB,
                arrayPointer = -1
            };
        }
    }
}
