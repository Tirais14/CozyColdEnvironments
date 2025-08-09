#nullable enable

using System.Collections.Generic;

namespace UTIRLib.Collections
{
    public class LinkedListNode<T> : IListNode<T>
    {
        public T? Value { get; set; }
        public IListNode<T>? NextNode { get; set; }
        public bool HasValue => EqualityComparer<T>.Default.Equals(Value!, default!);
        public bool HasNextNode => NextNode != null;

        IReadOnlyListNode<T>? IReadOnlyListNode<T>.NextNode => NextNode;

        public LinkedListNode(T? value = default, IListNode<T>? nextNode = null)
        {
            Value = value;
            NextNode = nextNode;
        }
    }
}