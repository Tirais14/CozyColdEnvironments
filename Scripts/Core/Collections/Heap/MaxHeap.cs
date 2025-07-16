using System;
using System.Collections.Generic;

#nullable enable

namespace UTIRLib.Collections
{
    public class MaxHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
        public MaxHeap() : base()
        { }

        public MaxHeap(IComparer<T> comparer) : base(comparer)
        { }

        public MaxHeap(int capacity) : base(capacity)
        { }

        public MaxHeap(int capacity, IComparer<T> comparer) : base(capacity, comparer)
        { }

        protected override void SiftUp(int index)
        {
            T child;
            while (index > 0)
            {
                child = tree[index];
                if (TryGetParent(index, out T? parent, out int parentIdx) &&
                    Compare(child, parent) <= 0)
                {
                    break;
                }

                Swap(index, parentIdx);
                index = parentIdx;
            }
        }

        protected override void SiftDown(int idx)
        {
            int largestChildIdx;
            int nodesCount = tree.Count;
            T largestChild;

            while (idx < nodesCount)
            {
                largestChildIdx = idx;
                largestChild = tree[largestChildIdx];

                if (TryGetLeftChild(idx, out T? leftChild, out int leftChildIdx) &&
                    Compare(leftChild, largestChild) > 0)
                {
                    largestChildIdx = leftChildIdx;
                    largestChild = tree[largestChildIdx];
                }

                if (TryGetRightChild(idx, out T? rightChild, out int rightChildIdx) &&
                    Compare(rightChild, largestChild) > 0)
                {
                    largestChildIdx = rightChildIdx;
                }

                if (largestChildIdx != idx)
                {
                    Swap(idx, largestChildIdx);
                    idx = largestChildIdx;
                }
                else { break; }
            }
        }
    }
}