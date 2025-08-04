#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public class ItemSlot : IItemSlot
    {
        private readonly IItemStack itemStack;
        private int capacityLimit;

        public IStorageItem Item => itemStack.Item;
        public int CapacityLimit {
            get => capacityLimit;
            set
            {
                if (value > itemStack.MaxItemCount)
                {
                    TirLibDebug.PrintWarning($"Capacity limit > {nameof(itemStack)}.{nameof(itemStack.MaxItemCount)}");
                    capacityLimit = itemStack.MaxItemCount;
                    return;
                }

                capacityLimit = value;
            }
        }
        public bool HasCapacityLimit => capacityLimit > 0;
        public int MaxItemCount {
            get
            {
                if (!HasCapacityLimit)
                    return itemStack.MaxItemCount;

                return capacityLimit;
            }
        }
        public bool HasItem => itemStack.HasItem;
        public bool IsContainerFull {
            get
            {
                if (HasCapacityLimit)
                    return itemStack.ItemCount >= CapacityLimit;

                return itemStack.IsContainerFull;
            }
        }
        public int ItemCount => itemStack.ItemCount;

        public ItemSlot(IItemStack itemStack, int capacityLimit = 0)
        {
            this.itemStack = itemStack;
            CapacityLimit = capacityLimit;
        }

        public IItemStack AddItem(IStorageItem item, int count)
        {
            count = ItemContainerHelper.CalulcateAddItemCount(this, count);

            return itemStack.AddItem(item, count);
        }

        public void AddItemFrom(IItemStack itemStack, int count)
        {
            count = ItemContainerHelper.CalulcateAddItemCount(this, count);

            this.itemStack.AddItemFrom(itemStack, count);
        }
        public void AddItemFrom(IItemStack itemStack)
        {
            this.itemStack.AddItemFrom(itemStack);
        }

        public IItemStack TakeItem(int count) => itemStack.TakeItem(count);

        public IItemStack TakeItemAll() => TakeItem(ItemCount);

        public bool Contains(IStorageItem item) => itemStack.Contains(item);
        public bool Contains(IItemStack itemStack)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));

            return this.itemStack.Equals(itemStack);
        }

        public bool CanHold(IStorageItem item) => itemStack.CanHold(item);

        public bool IsSameItem(IStorageItem item) => itemStack.IsSameItem(item);

        public void Clear() => itemStack.Clear();
    }
    public class ItemSlot<T> : IItemSlot<T>
        where T : IItemStack
    {
        private readonly ItemSlot itemSlot;

        public IStorageItem Item => itemSlot.Item;
        public int CapacityLimit {
            get => itemSlot.CapacityLimit;
            set => itemSlot.CapacityLimit = value;
        }
        public bool HasCapacityLimit => itemSlot.HasCapacityLimit;
        public int MaxItemCount => itemSlot.MaxItemCount;
        public bool HasItem => itemSlot.HasItem;
        public bool IsContainerFull => itemSlot.IsContainerFull;
        public int ItemCount => itemSlot.ItemCount;

        public ItemSlot(T itemStack, int capacityLimit = 0)
        {
            itemSlot = new ItemSlot(itemStack, capacityLimit);
            CapacityLimit = capacityLimit;
        }

        public T AddItem(IStorageItem item, int count)
        {
            return (T)itemSlot.AddItem(item, count);
        }

        public void AddItemFrom(T itemStack, int count)
        {
            itemSlot.AddItemFrom(itemStack, count);
        }
        public void AddItemFrom(T itemStack)
        {
            itemSlot.AddItemFrom(itemStack);
        }

        public T TakeItem(int count) => (T)itemSlot.TakeItem(count);

        public T TakeItemAll() => (T)itemSlot.TakeItemAll();

        public bool Contains(T itemStack) => itemSlot.Contains(itemStack);
        public bool Contains(IStorageItem item) => itemSlot.Contains(item);

        public bool CanHold(IStorageItem item) => itemSlot.CanHold(item);

        /// <exception cref="ArgumentNullException"></exception>
        public bool IsSameItem(IStorageItem item)
        {
            return itemSlot.IsSameItem(item);
        }

        public void Clear() => itemSlot.Clear();
    }
}
