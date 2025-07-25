#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public interface IItemStorageUI
    {
        int SlotCount { get; }
        IItemSlotUI this[int index] { get; }

        IItemSlotUI GetItemSlot(int index);

        void AddItemSlot(IItemSlotUI itemSlot);

        void RemoveItemSlotAt(int index);

        void AddItem(IItemUI item, int count);

        IItemSlotUI GetEmptySlot();

        IItemSlotUI GetSuitableSlot();
    }
}
