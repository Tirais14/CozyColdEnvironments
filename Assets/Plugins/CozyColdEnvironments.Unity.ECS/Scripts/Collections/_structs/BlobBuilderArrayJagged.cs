#nullable enable
using System.Collections.Generic;
using Unity.Entities;

namespace CCEnvs.UnityX.ECS.Collections
{
    public ref struct BlobBuilderArrayJagged<T>
        where T : struct
    {
        public BlobBuilderArray<T> ValuesAB;
        public BlobBuilderArray<int> LengthsAB;
        public BlobBuilderArray<int> OffsetsAB;

        public void Add(int arrayIndex, IReadOnlyList<T> chunk)
        {
            CC.Guard.IsNotNull(chunk, nameof(chunk));

            LengthsAB[arrayIndex] = chunk.Count;
            OffsetsAB[arrayIndex] = arrayIndex + 1;
        }

        public void Add(int arrayIndex, T value)
        {

        }
    }

    public static class BlobBuilderExtensions
    {
        public static BlobBuilderArrayJagged<T> AllocateJagged2D<T>(
            this in BlobBuilder builder,
            ref BlobArrayJagged<T> array,
            int length1,
            int valueCount
            )
            where T : struct
        {
            BlobBuilderArray<T> valuesAB = builder.Allocate(ref array.Values, valueCount);
            BlobBuilderArray<int> offsetsAB = builder.Allocate(ref array.Offsets, length1);
            BlobBuilderArray<int> lengthsAB = builder.Allocate(ref array.Lengths, length1);

            return new BlobBuilderArrayJagged<T>
            {
                ValuesAB = valuesAB,
                OffsetsAB = offsetsAB,
                LengthsAB = lengthsAB
            };
        }
    }
}
