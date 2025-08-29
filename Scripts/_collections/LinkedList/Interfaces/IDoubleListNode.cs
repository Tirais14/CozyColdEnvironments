#nullable enable

namespace CozyColdEnvironments.Collections
{
    public interface IDoubleListNode<T> : IReadOnlyDoubleListNode<T>, IListNode<T>
    {
        new IListNode<T>? PreviousNode { get; set; }
    }
}