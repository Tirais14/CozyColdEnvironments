#nullable enable

namespace UTIRLib.Collections
{
    public interface IReadOnlyListNode<out T>
    {
        T? Value { get; }
        IReadOnlyListNode<T>? NextNode { get; }
        bool HasValue { get; }
        bool HasNextNode { get; }
    }
}