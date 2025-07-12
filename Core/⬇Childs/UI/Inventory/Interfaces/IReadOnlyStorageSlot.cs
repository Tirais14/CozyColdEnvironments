#nullable enable

namespace UTIRLib.UI
{
    public interface IReadOnlyStorageSlot : IReadOnlyItemStackProvider
    {
        int Quantity { get; }
        bool IsEmpty { get; }
        bool IsNotEmpty => !IsEmpty;
        bool IsFull { get; }

        bool Contains(IItem item);
    }
}