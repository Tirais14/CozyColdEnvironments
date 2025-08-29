#nullable enable

namespace CozyColdEnvironments.Collections
{
    public interface IListNode<T> : IReadOnlyListNode<T>
    {
        new T? Value { get; set; }
        new IListNode<T>? NextNode { get; set; }
    }
}