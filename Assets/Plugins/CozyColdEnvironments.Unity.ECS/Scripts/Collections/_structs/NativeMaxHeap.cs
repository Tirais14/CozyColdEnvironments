using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;

namespace CCEnvs.UnityX.ECS.Collections
{
    public struct NativeMaxHeap<T, TComparer> : IDisposable
        where T : unmanaged
        where TComparer : unmanaged, IComparer<T>
    {
        private readonly struct ReverseComparer : IComparer<T>
        {
            private readonly TComparer _inner;
            public ReverseComparer(TComparer inner) => _inner = inner;
            public readonly int Compare(T x, T y) => _inner.Compare(y, x);
        }

        private NativeHeapCore<T, ReverseComparer> _core;

        public NativeMaxHeap(int initialCapacity, AllocatorManager.AllocatorHandle allocator, TComparer comparer)
        {
            _core = new NativeHeapCore<T, ReverseComparer>(
                initialCapacity,
                allocator,
                new ReverseComparer(comparer)
            );
        }

        public readonly int Count => _core.Count;
        public readonly int Capacity => _core.Capacity;
        public readonly bool IsCreated => _core.IsCreated;

        public void Push(T item) => _core.Push(item);
        public bool TryPop(out T item) => _core.TryPop(out item);
        public bool TryPeek(out T item) => _core.TryPeek(out item);

        public int IndexOf(T item) => _core.IndexOf(item);
        public bool Contains(T item) => _core.Contains(item);

        public T this[int index]
        {
            get => _core[index];
            set => _core[index] = value;
        }

        public T ReplaceAt(int index, T newItem) => _core.ReplaceAt(index, newItem);
        public bool TryReplace(T oldItem, T newItem) => _core.TryReplace(oldItem, newItem);

        public void Clear() => _core.Clear();
        public void Dispose() => _core.Dispose();
        public void Dispose(JobHandle inputDep) => _core.Dispose(inputDep);
    }
}