using System;
using System.Collections.Generic;

#nullable enable

namespace UTIRLib.Collections
{
    public class MinHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
        public MinHeap() : base()
        { }

        public MinHeap(IComparer<T> comparer) : base(comparer)
        { }

        public MinHeap(int capacity) : base(capacity)
        { }

        public MinHeap(int capacity, IComparer<T> comparer) : base(capacity, comparer)
        { }

        protected override void SiftUp(int index)
        {
            T child;
            while (index > 0)
            {
                child = tree[index];
                if (TryGetParent(index, out T? parent, out int parentIdx) && Compare(child, parent) >= 0)
                {
                    break;
                }

                Swap(index, parentIdx);
                index = parentIdx;
            }
        }

        protected override void SiftDown(int idx)
        {
            int smallestChildIdx;
            int nodesCount = tree.Count;
            T smallestChild;

            while (idx < nodesCount && idx >= 0)
            {
                smallestChildIdx = idx;
                smallestChild = tree[smallestChildIdx];

                if (TryGetLeftChild(idx, out T? leftChild, out int leftChildIdx) &&
                    Compare(leftChild, smallestChild) < 0)
                {
                    smallestChildIdx = leftChildIdx;
                    smallestChild = tree[smallestChildIdx];
                }

                if (TryGetRightChild(idx, out T? rightChild, out int rightChildIdx) &&
                    Compare(rightChild, smallestChild) < 0)
                {
                    smallestChildIdx = rightChildIdx;
                }

                if (smallestChildIdx != idx)
                {
                    Swap(idx, smallestChildIdx);
                    idx++;
                }
                else break;
            }
        }
    }
}