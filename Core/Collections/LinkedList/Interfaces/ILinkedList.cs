#nullable enable

using System.Collections.Generic;

namespace UTIRLib.Collections
{
    public interface ILinkedList<T> : IList<T>
    {
        IListNode<T>? FirstNode { get; set; }
        IReadOnlyListNode<T>? LastNode { get; }

        public IListNode<T> GetNode(int index);

        public T? GetValue(int index);

        public void SetNode(int index, IListNode<T> newNode);

        void SetValue(int index, T value);
    }
}