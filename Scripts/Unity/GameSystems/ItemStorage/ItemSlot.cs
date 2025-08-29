#nullable enable
using System;
using CCEnvs.Diagnostics;

namespace CCEnvs.GameSystems.Storages
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

        public ItemSlot(IItemStack itemStack)
        {
            this.itemStack = itemStack;
        }

        public IItemContainer AddItem(IStorageItem item, int count)
        {
            count = ItemContainerHelper.CalulcateAddItemCount(this, count);

            return itemStack.AddItem(item, count);
        }

        public void AddItemFrom(IItemContainer itemContainer, int count)
        {
            count = ItemContainerHelper.CalulcateAddItemCount(this, count);

            itemStack.AddItemFrom(itemContainer, count);
        }
        public void AddItemFrom(IItemContainer itemContainer)
        {
            itemStack.AddItemFrom(itemContainer);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void CopyItemFrom(IItemContainer itemContainer)
        {
            if (itemContainer.IsNull())
                throw new ArgumentNullException(nameof(itemContainer));
            if (!itemContainer.HasItem)
                return;

            AddItem(itemContainer.Item, itemContainer.ItemCount);
        }

        public IItemContainer TakeItem(int count) => itemStack.TakeItem(count);

        public IItemContainer TakeItemAll() => TakeItem(ItemCount);

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

        public ItemSlot(T itemStack)
        {
            itemSlot = new ItemSlot(itemStack);
        }

        public T AddItem(IStorageItem item, int count)
        {
            return (T)itemSlot.AddItem(item, count);
        }

        public void AddItemFrom(T itemContainer, int count)
        {
            itemSlot.AddItemFrom(itemContainer, count);
        }
        public void AddItemFrom(T itemContainer)
        {
            itemSlot.AddItemFrom(itemContainer);
        }

        public void CopyItemFrom(T itemContainer)
        {
            itemSlot.CopyItemFrom(itemContainer);
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
