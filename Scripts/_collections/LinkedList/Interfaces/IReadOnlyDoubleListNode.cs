#nullable enable

namespace CCEnvs.Collections
{
    public interface IReadOnlyDoubleListNode<out T> : IReadOnlyListNode<T>
    {
        IReadOnlyDoubleListNode<T>? PreviousNode { get; }
        bool HasPreviousNode { get; }
    }
}