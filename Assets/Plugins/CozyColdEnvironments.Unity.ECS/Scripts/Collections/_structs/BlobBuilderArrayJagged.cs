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

        private int arrayPointer;

        public void Add(IReadOnlyList<T> chunk)
        {
            CC.Guard.IsNotNull(chunk, nameof(chunk));

            LengthsAB[arrayPointer] = chunk.Count;

            int offset = 0;

            for (int i = 0; i < arrayPointer; i++)
                offset += LengthsAB[i];

            OffsetsAB[arrayPointer] = offset;

            arrayPointer++;

            for (int i = 0; i < chunk.Count; i++)
                ValuesAB[i + offset] = chunk[i];
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
                LengthsAB = lengthsAB
            };
        }
    }
}
