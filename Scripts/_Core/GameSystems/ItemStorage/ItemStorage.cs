using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.GameSystems.Storage
{
    public class ItemStorage<T> : IItemStorage<T>
        where T : IItemSlot
    {
        protected readonly List<T> slots;

        public int SlotCount => slots.Count;

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slots[index];
        }

        public ItemStorage(T[] slots)
        {
            this.slots = new List<T>(slots);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetItemSlot(int index) => slots[index];

        /// <exception cref="ArgumentNullException"></exception>
        public void AddItemSlot(T itemSlot)
        {
            if (itemSlot.IsNull())
                throw new ArgumentNullException(nameof(itemSlot));

            slots.Add(itemSlot);
        }

        public void RemoveItemSlotAt(int index)
        {
            slots.RemoveAt(index);
        }

        public bool HasItemStack(IItemStack? itemStack)
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

        public bool HasItem(IItem item, int count = 1)
        {
            IEnumerable<IItemStack> filteredStack = slots.Select(x => x.ItemStack)
                                                         .Where(x => !x.IsEmpty)
                                                         .Where(x => x.Item.Equals(item));
            int totalCount = 0;
            foreach (var stack in filteredStack)
            {
                totalCount += stack.ItemCount;

                if (totalCount >= count)
                    return true;
            }

            return false;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItemStack itemStack)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (slots.IsEmpty())
                throw new ArgumentException("Storage doesn't contain any slot.");
            if (HasItemStack(itemStack))
                throw new ArgumentException("Cannot add items to storage from itself item stack.");
            if (itemStack.IsEmpty)
                return;

            var suitableSlots = new Collection<IItemSlot>(
                x => x.IsNotNull() && !itemStack.IsEmpty,
                () => GetSuitableSlot(itemStack.Item)
                );

            foreach (var slot in suitableSlots)
                slot.ItemStack.AddItem(itemStack, itemStack.ItemCount);
        }
        
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItem item, int count)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            var itemStack = new ItemStack(item, count);
            AddItem(itemStack);
        }

        public T? GetEmptySlot()
        {
            return slots.Find(x => x.ItemStack.IsEmpty);
        }

        public bool TryGetEmptySlot([NotNullWhen(true)] out T? result)
        {
            result = GetEmptySlot();

            return result.IsNotNull();
        }

        /// <summary>
        /// Searchs not full slot with item or empty slot
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public T? GetSuitableSlot(IItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            T? empty = default;

            IItemStack itemStack;
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
        public bool TryGetSuitableSlot(IItem item,
                                       [NotNullWhen(true)] out T? result)
        {
            result = GetSuitableSlot(item);

            return result.IsNotNull();
        }
    }
}
