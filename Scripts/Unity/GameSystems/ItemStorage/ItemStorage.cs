using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.GameSystems.Storages
{
    public class ItemStorage : IItemStorage
    {
        protected readonly List<IItemSlot> slots;

        public int SlotCount => slots.Count;
        public IItemSlot this[int id] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => slots[id];
        }

        int IReadOnlyCollection<IItemSlot>.Count => slots.Count;

        public ItemStorage(IItemSlot[] slots)
        {
            this.slots = new List<IItemSlot>(slots);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IItemSlot GetSlot(int id) => slots[id];

        /// <exception cref="ArgumentNullException"></exception>
        public void AddItemSlot(IItemSlot itemSlot)
        {
            if (itemSlot.IsNull())
                throw new ArgumentNullException(nameof(itemSlot));

            slots.Add(itemSlot);
        }

        public void RemoveSlot(int id) => slots.RemoveAt(id);

        public bool Contains(IItemStack? itemStack)
        {
            if (itemStack.IsNull())
                return false;

            int slotsCount = slots.Count;
            for (int i = 0; i < slotsCount; i++)
            {
                if (slots[i].Contains(itemStack))
                    return true;
            }

            return false;
        }

        public bool Contains(IItemSlot slot) => slots.Contains(slot);

        public bool Contains(IStorageItem item, int count = 1)
        {
            IEnumerable<IItemSlot> slotsWithItem = slots.Where(x => x.HasItem && x.IsSameItem(item));
            int totalCount = 0;
            foreach (var stack in slotsWithItem)
            {
                totalCount += stack.ItemCount;

                if (totalCount >= count)
                    return true;
            }

            return false;
        }

        public bool CanHold(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (slots.IsEmpty())
                throw new InvalidOperationException("Item cannot be validated in empty storage.");

            return slots[0].CanHold(item);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItemFrom(IItemStack itemStack)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (slots.IsEmpty())
                throw new ArgumentException("Storage doesn't contain any slot.");
            if (Contains(itemStack))
                throw new ArgumentException("Cannot add items to storage from itself item stack.");
            if (!itemStack.HasItem)
            {
                Debug.LogWarning("Try to move items from empty item stack.");
                return;
            }

            var loopPredicate = new LoopPredicate<IItemSlot?>(
                (x) => x.IsNotNull() && itemStack.HasItem);

            IItemSlot? currentSlot = GetSuitableSlot(itemStack.Item);
            while (loopPredicate.Invoke(currentSlot))
            {
                currentSlot!.AddItemFrom(itemStack);

                if (!itemStack.HasItem)
                    break;

                currentSlot = GetSuitableSlot(itemStack.Item);
            }
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IItemStack AddItem(IStorageItem item, int count)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            var itemStack = new ItemStack(item, count);
            AddItemFrom(itemStack);

            return itemStack;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public int GetSlotId(IItemSlot slot)
        {
            if (slot.IsNull())
                throw new ArgumentNullException(nameof(slot));

            return slots.IndexOf(slot);
        }

        public IItemSlot? GetEmptySlot()
        {
            return slots.Find(x => !x.HasItem);
        }

        public bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlot? result)
        {
            result = GetEmptySlot();

            return result.IsNotNull();
        }

        /// <summary>
        /// Searchs not full slot with item or empty slot
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IItemSlot? GetSuitableSlot(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            IItemSlot? empty = default;

            IItemSlot itemSlot;
            int slotsCount = slots.Count;
            for (int i = 0; i < slotsCount; i++)
            {
                itemSlot = slots[i];

                if (!itemSlot.HasItem)
                {
                    if (empty.IsNull())
                        empty = slots[i];
                    else
                        continue;
                }

                if (itemSlot.IsSameItem(item) && !itemSlot.IsContainerFull)
                    return slots[i];
            }

            return empty;
        }

        /// <summary>
        /// Searchs not full slot with item or empty slot
        /// </summary>
        public bool TryGetSuitableSlot(IStorageItem item,
                                       [NotNullWhen(true)] out IItemSlot? result)
        {
            result = GetSuitableSlot(item);

            return result.IsNotNull();
        }

        public IEnumerator<IItemSlot> GetEnumerator() => slots.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ItemStorage<T> : IItemStorage<T>
        where T : IItemSlot
    {
        protected readonly ItemStorage storage;

        public T this[int id] => (T)storage[id];
        public int SlotCount => storage.SlotCount;

        int IReadOnlyCollection<IItemSlot>.Count => storage.SlotCount;

        /// <summary>
        /// For <see cref="InstanceFactory"/>
        /// </summary>
        protected ItemStorage(IItemSlot[] slots)
        {
            storage = new ItemStorage(slots);
        }
        public ItemStorage(T[] slots)
        {
            storage = new ItemStorage(slots.Cast<IItemSlot>().ToArray());
        }

        public IItemStack AddItem(IStorageItem item, int count)
        {
            return storage.AddItem(item, count);
        }

        public void AddItemFrom(IItemStack itemStack) => storage.AddItemFrom(itemStack);

        public void AddItemSlot(T itemSlot) => storage.AddItemSlot(itemSlot);

        public bool CanHold(IStorageItem item) => storage.CanHold(item);

        public bool Contains(T slot) => storage.Contains(slot);

        public bool Contains(IItemStack itemStack) => storage.Contains(itemStack);

        public T? GetEmptySlot() => (T?)storage.GetEmptySlot();

        public T GetSlot(int id) => (T)storage.GetSlot(id);

        public int GetSlotId(T slot) => storage.GetSlotId(slot);

        public T? GetSuitableSlot(IStorageItem item) => (T?)storage.GetSuitableSlot(item);

        public bool Contains(IStorageItem item, int count = 1)
        {
            return storage.Contains(item, count);
        }

        public void RemoveSlot(int id) => storage.RemoveSlot(id);

        public bool TryGetEmptySlot([NotNullWhen(true)] out T? result)
        {
            bool success = storage.TryGetEmptySlot(out IItemSlot? nonTyped);

            result = (T?)nonTyped;

            return success;
        }

        public bool TryGetSuitableSlot(IStorageItem item,
                                       [NotNullWhen(true)] out T? result)
        {
            bool success = storage.TryGetSuitableSlot(item, out IItemSlot ? nonTyped);

            result = (T?)nonTyped;

            return success;
        }

        public IEnumerator<IItemSlot> GetEnumerator() => storage.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
