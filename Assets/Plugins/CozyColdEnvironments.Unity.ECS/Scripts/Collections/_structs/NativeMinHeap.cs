using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;

namespace CCEnvs.UnityX.ECS.Collections
{
    public struct NativeMinHeap<T, TComparer> : IDisposable
        where T : unmanaged
        where TComparer : unmanaged, IComparer<T>
    {
        private NativeHeapCore<T, TComparer> _core;

        public NativeMinHeap(int initialCapacity, AllocatorManager.AllocatorHandle allocator, TComparer comparer)
        {
            _core = new NativeHeapCore<T, TComparer>(initialCapacity, allocator, comparer);
        }

        public readonly int Count => _core.Count;
        public readonly int Capacity => _core.Capacity;
        public readonly bool IsCreated => _core.IsCreated;

        public void Push(T item) => _core.Push(item);
        public bool TryPop(out T item) => _core.TryPop(out item);
        public bool TryPeek(out T item) => _core.TryPeek(out item);
        public void Clear() => _core.Clear();
        public void Dispose() => _core.Dispose();
        public void Dispose(JobHandle inputDep) => _core.Dispose(inputDep);
    }
}