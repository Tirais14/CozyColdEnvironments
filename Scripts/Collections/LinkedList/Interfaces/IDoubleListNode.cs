#nullable enable

namespace UTIRLib.Collections
{
    public interface IDoubleListNode<T> : IReadOnlyDoubleListNode<T>, IListNode<T>
    {
        new IListNode<T>? PreviousNode { get; set; }
    }
}