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
        public static JobHandle DisposeNested<TKey, TValue>(in NativeHashMap<TKey, NativeList<TValue>> source, JobHandle dependency)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            if (!source.IsCreated || source.IsEmpty)
                return dependency;

            var handles = new NativeList<JobHandle>(source.Count * 32, Allocator.Temp);

            foreach (var kvp in source)
            {
                JobHandle handle = kvp.Value.Dispose(dependency);
                handles.Add(handle);
            }

            handles.Add(source.Dispose(dependency));

            var combinedHandle = JobHandle.CombineDependencies(handles.AsArray());

            handles.Dispose();

            return combinedHandle;
        }

        [BurstCompile]
        public static void DisposeNested<TKey, TValue>(in NativeHashMap<TKey, NativeHashSet<TValue>> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged, IEquatable<TValue>
        {
            foreach (var kvp in source)
                kvp.Value.Dispose();

            source.Dispose();
        }

        [BurstCompile]
        public static JobHandle DisposeNested<TKey, TValue>(in NativeHashMap<TKey, NativeHashSet<TValue>> source, JobHandle dependency)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged, IEquatable<TValue>
        {
            if (!source.IsCreated || source.IsEmpty)
                return dependency;

            var handles = new NativeList<JobHandle>(source.Count * 32, Allocator.Temp);

            foreach (var kvp in source)
            {
                JobHandle handle = kvp.Value.Dispose(dependency);
                handles.Add(handle);
            }

            handles.Add(source.Dispose(dependency));

            var combinedHandle = JobHandle.CombineDependencies(handles.AsArray());

            handles.Dispose();

            return combinedHandle;
        }

        [BurstCompile]
        public static void DisposeNested<TKey, TKey2, TValue>(in NativeHashMap<TKey, NativeHashMap<TKey2, TValue>> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TKey2 : unmanaged, IEquatable<TKey2>
            where TValue : unmanaged
        {
            foreach (var kvp in source)
                kvp.Value.Dispose();

            source.Dispose();
        }

        [BurstCompile]
        public static JobHandle DisposeNested<TKey, TKey2, TValue>(in NativeHashMap<TKey, NativeHashMap<TKey2, TValue>> source, JobHandle dependency)
            where TKey : unmanaged, IEquatable<TKey>
            where TKey2 : unmanaged, IEquatable<TKey2>
            where TValue : unmanaged
        {
            if (!source.IsCreated || source.IsEmpty)
                return dependency;

            var handles = new NativeList<JobHandle>(source.Count * 32, Allocator.Temp);

            foreach (var kvp in source)
            {
                JobHandle handle = kvp.Value.Dispose(dependency);
                handles.Add(handle);
            }

            handles.Add(source.Dispose(dependency));

            var combinedHandle = JobHandle.CombineDependencies(handles.AsArray());

            handles.Dispose();

            return combinedHandle;
        }

    }
}
