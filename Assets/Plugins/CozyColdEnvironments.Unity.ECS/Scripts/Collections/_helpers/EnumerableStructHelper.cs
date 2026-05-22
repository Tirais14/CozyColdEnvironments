using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    public static class EnumerableStructHelper
    {
        public static NativeList<T> ToNativeList<T>(this NativeArray<T> source, Allocator allocator)
            where T : unmanaged
        {
            var list = new NativeList<T>(allocator);

            for (int i = 0; i < source.Length; i++)
                list.Add(source[i]);

            return list;
        }

        public static NativeList<T> ToNativeList<T>(this DynamicBuffer<T> source, Allocator allocator)
            where T : unmanaged
        {
            var list = new NativeList<T>(allocator);

            for (int i = 0; i < source.Length; i++)
                list.Add(source[i]);

            return list;
        }
    }
}
