#nullable enable

namespace CozyColdEnvironments.Collections
{
    public interface IReadOnlyDoubleListNode<out T> : IReadOnlyListNode<T>
    {
        IReadOnlyDoubleListNode<T>? PreviousNode { get; }
        bool HasPreviousNode { get; }
    }
}