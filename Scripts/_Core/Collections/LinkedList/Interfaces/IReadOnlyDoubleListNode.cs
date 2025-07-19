#nullable enable

namespace UTIRLib.Collections
{
    public interface IReadOnlyDoubleListNode<out T> : IReadOnlyListNode<T>
    {
        IReadOnlyDoubleListNode<T>? PreviousNode { get; }
        bool HasPreviousNode { get; }
    }
}