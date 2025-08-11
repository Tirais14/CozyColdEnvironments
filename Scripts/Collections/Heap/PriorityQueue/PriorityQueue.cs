using System;

#nullable enable

namespace UTIRLib.Collections
{
    public class PriorityQueue<TPriority, TValue>
        where TPriority : struct, IComparable<TPriority>
    {
        private readonly MinHeap<PriorityValuePair<TPriority, TValue>> heap;
        public int Count => heap.Count;

        public PriorityQueue() => heap = new MinHeap<PriorityValuePair<TPriority, TValue>>();

        public PriorityQueue(int capacity) => heap = new MinHeap<PriorityValuePair<TPriority, TValue>>(capacity);

        public void Clear() => heap.Clear();

        public bool Contains(TPriority priority, TValue value) =>
            heap.Contains(new PriorityValuePair<TPriority, TValue>(priority, value));

        public void Enqueue(TPriority priority, TValue value) =>
            heap.Insert(new PriorityValuePair<TPriority, TValue>(priority, value));

        public TValue Dequeue() => heap.Extract().value;
    }
}