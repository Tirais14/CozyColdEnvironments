#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace UTIRLib.Collections
{
    public class LinkedList<T> : ILinkedList<T>
    {
        protected int count;
        protected IListNode<T>? lastNode;

        public IListNode<T>? FirstNode { get; set; }

        public IReadOnlyListNode<T>? LastNode {
            get => lastNode;
        }

        public int Count => count;
        public bool IsReadOnly => false;
#nullable disable
        public T this[int index] { get => GetValue(index); set => SetValue(index, value); }
#nullable enable

        public LinkedList()
        {
        }

        public LinkedList(IEnumerable<T> values) : this()
        {
            foreach (T value in values)
            {
                Add(value);
            }
        }

        public IListNode<T> GetNode(int index)
        {
            IListNode<T> result;
            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                if (enumerator.Index == index)
                {
                    result = enumerator.Current;
                    return result;
                }
            }

            throw new IndexOutOfRangeException();
        }

        public T? GetValue(int index) => GetNode(index).Value;

        public void SetNode(int index, IListNode<T> newNode)
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                if (enumerator.Index == index)
                {
                    enumerator.Current = newNode;
                    return;
                }
            }
        }

        public void SetValue(int index, T value) => GetNode(index).Value = value;

        public int IndexOf(T item)
        {
            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                if (EqualityComparer<T>.Default.Equals(enumerator.Current.Value!, item))
                {
                    return enumerator.Index;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException();
            }

            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                if (enumerator.Index == index)
                {
                    enumerator.Current = new LinkedListNode<T>(item) {
                        NextNode = enumerator.Next
                    };

                    return;
                }
            }
        }

        public void Add(T item)
        {
            LinkedListNode<T> newNode = new(item);
            if (lastNode != null)
            {
                lastNode.NextNode = newNode;
            }
            else
            {
                FirstNode = newNode;
                lastNode = newNode;
            }
        }

        public bool Contains(T item) => IndexOf(item) > -1;

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (count + arrayIndex < array.Length)
            {
                throw new ArgumentException(nameof(array));
            }

            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                array[arrayIndex++] = enumerator.Current.Value!;
            }
        }

        public bool Remove(T? item)
        {
            if (EqualityComparer<T>.Default.Equals(item!, default!) || count == 0)
            {
                return false;
            }

            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                if (EqualityComparer<T>.Default.Equals(enumerator.Current.Value!, item))
                {
                    enumerator.Previous!.NextNode = enumerator.Next;
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index >= count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == 0)
            {
                FirstNode = FirstNode!.NextNode;
            }

            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                if (enumerator.Index == index)
                {
                    enumerator.Previous!.NextNode = enumerator.Next;
                }
            }
        }

        public void Clear() => FirstNode = null;

        public IEnumerator<T> GetEnumerator() => new ValueEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<IListNode<T>>
        {
            private readonly ILinkedList<T> linkedList;
            private IListNode<T> current;

            public IListNode<T>? Previous { get; private set; }

            public IListNode<T> Current {
                readonly get => current;
                set {
                    if (current == linkedList.FirstNode)
                    {
                        throw new NotSupportedException("First node cannot be setted by enumerator.");
                    }

                    IListNode<T> temp = current;
                    current = value ?? throw new ArgumentNullException(nameof(value));
                    current.NextNode = temp;
                    Previous!.NextNode = current;
                }
            }

            public readonly IListNode<T>? Next => current?.NextNode;
            public int Index { get; private set; }
            readonly object? IEnumerator.Current => Current;

            public Enumerator(ILinkedList<T> linkedList)
            {
                this.linkedList = linkedList;
                Index = 0;
                Previous = null;
                current = linkedList.FirstNode!;
            }

            public bool MoveNext()
            {
                if (current == null)
                {
                    return false;
                }

                Previous = current;
                current = current?.NextNode!;
                Index++;

                return Current != null;
            }

            public void Reset()
            {
                current = linkedList.FirstNode!;
                Index = 0;
            }

            public readonly void Dispose()
            { }
        }

        public readonly struct ValueEnumerator : IEnumerator<T>
        {
            private readonly Enumerator listEnumerator;

            public readonly T Current => listEnumerator.Current.Value!;
            readonly object? IEnumerator.Current => Current!;

            public ValueEnumerator(ILinkedList<T> linkedList) => listEnumerator = new Enumerator(linkedList);

            public readonly bool MoveNext() => listEnumerator.MoveNext();

            public readonly void Reset() => listEnumerator.Reset();

            public readonly void Dispose() => listEnumerator.Dispose();
        }
    }
}