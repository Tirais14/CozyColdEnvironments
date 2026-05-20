using Unity.Entities;

#nullable enable
namespace CCEnvs.Unity.ECS.Collections
{
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
    }
}
