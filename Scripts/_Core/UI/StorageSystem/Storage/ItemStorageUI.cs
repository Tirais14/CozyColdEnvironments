using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public bool HasItemStack(IItemStackUI? itemStack)
        {
            if (itemStack.IsNull())
                return false;

            int slotsCount = slots.Count;
            for (int i = 0; i < slotsCount; i++)
            {
                if (slots[i].ItemStack.Equals(itemStack))
                    return true;
            }

            return false;
        }

        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItemStackUI itemStack)
        {
            if (HasItemStack(itemStack))
                throw new ArgumentException("Cannot add items to storage from itself item stack.");
            if (itemStack.IsEmpty)
                return;

            var suitableSlots = new Collection<IItemSlotUI>(
                x => x.IsNotNull() && !itemStack.IsEmpty,
                () => GetSuitableSlot(itemStack.Item)
                );

            foreach (var slot in suitableSlots)
                slot.ItemStack.AddItem(itemStack, itemStack.ItemCount);
        }

        public IItemSlotUI? GetEmptySlot()
        {
            return slots.Find(x => x.ItemStack.IsEmpty);
        }

        public bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlotUI? result)
        {
            result = GetEmptySlot();

            return result.IsNotNull();
        }

        /// <summary>
        /// Searchs not full slot with item or empty slot
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IItemSlotUI? GetSuitableSlot(IItemUI item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            IItemSlotUI? empty = null;

            IItemStackUI itemStack;
            int slotsCount = slots.Count;   
            for (int i = 0; i < slotsCount; i++)
            {
                itemStack = slots[i].ItemStack;

                if (empty.IsNull() && itemStack.IsEmpty)
                    empty = slots[i];

                if (itemStack.Item.Equals(item) && !itemStack.IsFull)
                    return slots[i];
            }

            return empty;
        }

        /// <summary>
        /// Searchs not full slot with item or empty slot
        /// </summary>
        public bool TryGetSuitableSlot(IItemUI item,
                                       [NotNullWhen(true)] out IItemSlotUI? result)
        {
            result = GetSuitableSlot(item);

            return result.IsNotNull();
        }
    }
}
