#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStorageUI
    {
        int SlotCount { get; }
        IItemSlotUI this[int index] { get; }

        IItemSlotUI GetItemSlot(int index);

        void AddItemSlot(IItemSlotUI itemSlot);

        void RemoveItemSlotAt(int index);
    }
}
