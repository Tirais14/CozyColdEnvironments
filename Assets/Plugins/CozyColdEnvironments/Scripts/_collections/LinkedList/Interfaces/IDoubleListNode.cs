#nullable enable

namespace CCEnvs.Collections
{
    public interface IDoubleListNode<T> : IReadOnlyDoubleListNode<T>, IListNode<T>
    {
        new IListNode<T>? PreviousNode { get; set; }
    }
}