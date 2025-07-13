#nullable enable

namespace UTIRLib.UI
{
    public interface IReadOnlyItemStack : IItemContainer
    {
        bool IsEmpty { get; }
        bool IsNotEmpty => !IsEmpty;
        bool IsFull { get; }
    }
}