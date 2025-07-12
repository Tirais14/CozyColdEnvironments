using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Collections
{
    public abstract class BinaryHeap<T> where T : IComparable<T>
    {
        private const int FirstElementIdx = 0;
        private const int ChildNodesCount = 2;
        private const int LeftChildIdxOffset = 1;
        private const int RightChildIdxOffset = 2;

        protected readonly List<T> tree = new();
        protected readonly IComparer<T>? comparer;
        public int Count => tree.Count;
        public int LastElementIdx => tree.Count - 1;
        public bool IsEmpty => tree.Count == 0;

        protected BinaryHeap()
        { }

        protected BinaryHeap(IComparer<T> comparer) => this.comparer = comparer;

        protected BinaryHeap(int capacity) => tree = new List<T>(capacity);

        protected BinaryHeap(int capacity, IComparer<T> comparer) : this(capacity) => this.comparer = comparer;

        public void Clear() => tree.Clear();

        public bool Contains(T value) => tree.Contains(value);

        public void Insert(T value)
        {
            tree.Add(value);
            SiftUp(LastElementIdx);
        }

        public T Extract()
        {
            if (IsEmpty) { throw new InvalidOperationException("Heap is empty."); }

            T root = GetRoot();
            SetRoot(tree[LastElementIdx]);
            tree.RemoveAt(LastElementIdx);
            SiftDown(FirstElementIdx);

            return root;
        }

        protected int Compare(T x, T y) => comparer?.Compare(x, y) ?? x.CompareTo(y);

        protected bool InRange(int index) => index >= 0 && index < tree.Count;

        protected void SetRoot(T root) => tree[FirstElementIdx] = root;

        protected T GetRoot() => tree[FirstElementIdx];

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected bool TryGetParent(int childIdx, [NotNullWhen(true)] out T? parent, out int parentIdx)
        {
            if (childIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(childIdx));
            }

            const int ParentOffset = 1;
            parent = default;
            parentIdx = (childIdx - ParentOffset) / 2;
            if (!InRange(parentIdx))
            {
                parentIdx = -1;
                return false;
            }

            parent = tree[parentIdx];
            return true;
        }

        protected bool TryGetLeftChild(int index, [NotNullWhen(true)] out T? leftChild, out int leftChildIdx)
        {
            if (index < 0) { throw new ArgumentOutOfRangeException("index"); }

            leftChild = default;
            leftChildIdx = ChildNodesCount * index + LeftChildIdxOffset;
            if (!InRange(leftChildIdx)) { return false; }

            leftChild = tree[leftChildIdx];
            return true;
        }

        protected bool TryGetRightChild(int index, [NotNullWhen(true)] out T? rightChild, out int rightChildIdx)
        {
            if (index < 0) { throw new ArgumentOutOfRangeException("index"); }

            rightChild = default;
            rightChildIdx = ChildNodesCount * index + RightChildIdxOffset;
            if (!InRange(rightChildIdx)) { return false; }

            rightChild = tree[rightChildIdx];
            return true;
        }

        protected void Swap(int idx1, int idx2) =>
            (tree[idx2], tree[idx1]) = (tree[idx1], tree[idx2]);

        protected abstract void SiftUp(int index);

        protected abstract void SiftDown(int idx);
    }
}