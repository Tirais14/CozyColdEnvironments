using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageUI : IItemStorageUI
    {
        protected readonly List<IItemSlotUI> slots;

        public int SlotCount => slots.Count;
        public IItemSlotUI this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slots[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IItemSlotUI GetItemSlot(int index) => slots[index];

        public ItemStorageUI(IItemSlotUI[] slots)
        {
            this.slots = new List<IItemSlotUI>(slots);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void AddItemSlot(IItemSlotUI itemSlot)
        {
            if (itemSlot.IsNull())
                throw new ArgumentNullException(nameof(itemSlot));

            slots.Add(itemSlot);
        }

        public void RemoveItemSlotAt(int index)
        {
            slots.RemoveAt(index);
        }

        public void AddItem(IItemUI item, int count)
        {
            var itemStack = new ItemStackUI(int.MaxValue, item, count);

            throw new NotImplementedException();
        }

        public IItemSlotUI GetEmptySlot()
        {
            throw new NotImplementedException();
        }

        public IItemSlotUI GetSuitableSlot()
        {
            throw new NotImplementedException();
        }
    }
}
