using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;

namespace CCEnvs.UnityX.ECS.Collections
{
    internal struct NativeHeapCore<T, TComparer> : IDisposable
        where T : unmanaged
        where TComparer : unmanaged, IComparer<T>
    {
        private NativeList<T> _items;
        private readonly TComparer _comparer;

        public readonly int Count => _items.Length;
        public readonly int Capacity => _items.Capacity;
        public readonly bool IsCreated => _items.IsCreated;

        public NativeHeapCore(int initialCapacity, AllocatorManager.AllocatorHandle allocator, TComparer comparer)
        {
            _items = new NativeList<T>(initialCapacity, allocator);
            _comparer = comparer;
        }

        public void Push(T item)
        {
            _items.Add(item);
            SiftUp(_items.Length - 1);
        }

        public bool TryPop(out T item)
        {
            int count = _items.Length;
            if (count == 0)
            {
                item = default;
                return false;
            }

            item = _items[0];
            int lastIndex = count - 1;
            _items[0] = _items[lastIndex];
            _items.RemoveAt(lastIndex);

            if (_items.Length > 0)
                SiftDown(0);

            return true;
        }

        public bool TryPeek(out T item)
        {
            if (_items.Length == 0)
            {
                item = default;
                return false;
            }

            item = _items[0];
            return true;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_comparer.Compare(_items[i], item) == 0)
                    return i;
            }
            return -1;
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public T this[int index]
        {
            get => _items[index];
            set => ReplaceAt(index, value);
        }

        /// <summary>
        /// Заменяет элемент по индексу с восстановлением свойств кучи.
        /// Возвращает старое значение.
        /// </summary>
        public T ReplaceAt(int index, T newItem)
        {
            if ((uint)index >= (uint)_items.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            T oldItem = _items[index];
            int cmp = _comparer.Compare(newItem, oldItem);
            _items[index] = newItem;

            if (cmp < 0)
                SiftUp(index);
            else if (cmp > 0)
                SiftDown(index);

            return oldItem;
        }

        /// <summary>
        /// Ищет элемент, эквивалентный <paramref name="oldItem"/> (по компаратору),
        /// и заменяет его на <paramref name="newItem"/>.
        /// </summary>
        public bool TryReplace(T oldItem, T newItem)
        {
            int index = IndexOf(oldItem);
            if (index < 0)
                return false;

            ReplaceAt(index, newItem);
            return true;
        }

        public void Clear() => _items.Clear();

        public void Dispose()
        {
            if (_items.IsCreated)
                _items.Dispose();
        }

        public void Dispose(JobHandle handle)
        {
            _items.Dispose(handle);
        }

        private void SiftUp(int index)
        {
            var item = _items[index];
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_comparer.Compare(item, _items[parentIndex]) >= 0)
                    break;

                _items[index] = _items[parentIndex];
                index = parentIndex;
            }
            _items[index] = item;
        }

        private void SiftDown(int index)
        {
            var item = _items[index];
            int count = _items.Length;
            int half = count / 2;
            while (index < half)
            {
                int childIndex = 2 * index + 1;
                int rightIndex = childIndex + 1;

                if (rightIndex < count && _comparer.Compare(_items[rightIndex], _items[childIndex]) < 0)
                    childIndex = rightIndex;

                if (_comparer.Compare(item, _items[childIndex]) <= 0)
                    break;

                _items[index] = _items[childIndex];
                index = childIndex;
            }
            _items[index] = item;
        }
    }
}