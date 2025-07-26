#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace UTIRLib.GameSystems.Storage
{
    public interface IItemStorage
    {
        int SlotCount { get; }
        IItemSlot this[int index] { get; }

        IItemSlot GetItemSlot(int index);

        void AddItemSlot(IItemSlot itemSlot);

        void RemoveItemSlotAt(int index);

        void AddItem(IItemStack itemStack);

        bool HasItemStack(IItemStack itemStack);

        IItemSlot? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlot? result);

        IItemSlot? GetSuitableSlot(IItem item);

        bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out IItemSlot? result);
    }
}
