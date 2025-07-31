using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using UTIRLib.Reflection.Cached;

#nullable enable
namespace UTIRLib.GameSystems.Storage
{
    public class ItemStorage<T> : IItemStorage<T>
        where T : IItemSlot
    {
        protected readonly List<T> slots;

        public int SlotCount => slots.Count;

        public T this[int id] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slots[id];
        }

        public ItemStorage(T[] slots)
        {
            this.slots = new List<T>(slots);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetSlot(int id) => slots[id];

        /// <exception cref="ArgumentNullException"></exception>
        public void AddItemSlot(T itemSlot)
        {
            if (itemSlot.IsNull())
                throw new ArgumentNullException(nameof(itemSlot));

            slots.Add(itemSlot);
        }

        public void RemoveSlot(int id)
        {
            slots.RemoveAt(id);
        }

        public bool Contains(IItemStack? itemStack)
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

        public bool Contains(T slot)
        {
            return slots.Contains(slot);
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

        public bool CanHold(IItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (slots.IsEmpty())
                throw new InvalidOperationException("Item cannot be validated in empty storage.");

            if (slots.Find(x => x.ItemStack.IsEmpty) is T notEmptySlot)
                return item.GetType().IsType(notEmptySlot.ItemStack.Item.GetType());

            FieldInfo itemStackField = TypeCache.GetField(typeof(T), nameof(IItemSlot.ItemStack));
            FieldInfo itemField = TypeCache.GetField(itemStackField.FieldType, nameof(IItemStack.Item));

            return itemField.FieldType.IsType(item.GetType());
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItemStack itemStack)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (slots.IsEmpty())
                throw new ArgumentException("Storage doesn't contain any slot.");
            if (Contains(itemStack))
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
        public IItemStack AddItem(IItem item, int count)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            var itemStack = new ItemStack<IItem>(item, count);
            AddItem(itemStack);

            return itemStack;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public int GetSlotId(T slot)
        {
            if (slot.IsNull())
                throw new ArgumentNullException(nameof(slot));

            return slots.IndexOf(slot);
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

        public IEnumerator<T> GetEnumerator() => slots.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
