using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.UI
{
    public interface IStorage : IReadOnlyList<IStorageSlot>, IOpenable
    {
        int SlotQuantity { get; set; }

        IItemStack AddItem(IItem item, int quantity);

        void AddItemStack(IItemStack itemStack);

        void SetSlotQuantity(int quantity);

        IStorageSlot GetSlot(int id);

        IStorageSlot? GetSuitableSlot(IItem item);

        bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out IStorageSlot? slot);

        void AddSlots(int quantity = 1);

        void RemoveSlots(int quantity = 1);

        void Clear();

        void ClearSlots();
    }
}