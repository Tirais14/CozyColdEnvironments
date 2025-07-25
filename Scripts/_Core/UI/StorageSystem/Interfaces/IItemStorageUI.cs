#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace UTIRLib.UI.StorageSystem
{
    public interface IItemStorageUI
    {
        int SlotCount { get; }
        IItemSlotUI this[int index] { get; }

        IItemSlotUI GetItemSlot(int index);

        void AddItemSlot(IItemSlotUI itemSlot);

        void RemoveItemSlotAt(int index);

        void AddItem(IItemStackUI itemStack);

        bool HasItemStack(IItemStackUI itemStack);

        IItemSlotUI? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlotUI? result);

        IItemSlotUI? GetSuitableSlot(IItemUI item);

        bool TryGetSuitableSlot(IItemUI item, [NotNullWhen(true)] out IItemSlotUI? result);
    }
}
