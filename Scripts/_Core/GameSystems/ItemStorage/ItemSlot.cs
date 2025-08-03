#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public class ItemSlot : IItemSlot
    {
        public IItemStack ItemStack { get; private set; }
        public int CapacityLimit { get; set; }
        public bool HasCapacityLimit => CapacityLimit > 0;
        public bool IsEmpty => ItemStack.IsEmpty;
        public bool IsFull {
            get
            {
                if (HasCapacityLimit)
                    return ItemStack.ItemCount >= CapacityLimit;

                return ItemStack.IsFull;
            }
        }
        public int ItemCount => ItemStack.ItemCount;

        public ItemSlot(IItemStack itemStack, int capacityLimit = 0)
        {
            ItemStack = itemStack;
            CapacityLimit = capacityLimit;
        }

        public IItemStack AddItem(IStorageItem item, int count)
        {
            count = ItemSlotHelper.CalculateAddItemCount(this, count);

            return ItemStack.AddItem(item, count);
        }

        public void AddItemFrom(IItemStack itemStack, int count)
        {
            count = ItemSlotHelper.CalculateAddItemCount(this, count);

            ItemStack.AddItemFrom(itemStack, count);
        }

        public void AddItemFrom(IItemStack itemStack)
        {
            ItemStack.AddItemFrom(itemStack);
        }

        public IItemStack TakeItem(int count) => ItemStack.TakeItem(count);

        public IItemStack TakeItemAll() => TakeItem(ItemCount);

        /// <exception cref="ArgumentNullException"></exception>
        public bool IsSameItem(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            return ItemStack.Item.Equals(item);
        }
    }
    public class ItemSlot<T> : IItemSlot<T>
        where T : IItemStack
    {
        public T ItemStack { get; private set; }
        public int CapacityLimit { get; set; }
        public bool HasCapacityLimit => CapacityLimit > 0;
        public bool IsEmpty => ItemStack.IsEmpty;
        public bool IsFull {
            get
            {
                if (HasCapacityLimit)
                    return ItemStack.ItemCount >= CapacityLimit;

                return ItemStack.IsFull;
            }
        }
        public int ItemCount => ItemStack.ItemCount;

        public ItemSlot(T itemStack, int capacityLimit = 0)
        {
            ItemStack = itemStack;
            CapacityLimit = capacityLimit;
        }

        public T AddItem(IStorageItem item, int count)
        {
            count = ItemSlotHelper.CalculateAddItemCount(this, count);

            return (T)ItemStack.AddItem(item, count);
        }

        public void AddItemFrom(T itemStack, int count)
        {
            count = ItemSlotHelper.CalculateAddItemCount(this, count);

            ItemStack.AddItemFrom(itemStack, count);
        }

        public void AddItemFrom(T itemStack)
        {
            ItemStack.AddItemFrom(itemStack);
        }

        public T TakeItem(int count) => (T)ItemStack.TakeItem(count);

        public T TakeItemAll() => TakeItem(ItemCount);

        /// <exception cref="ArgumentNullException"></exception>
        public bool IsSameItem(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            return ItemStack.Item.Equals(item);
        }
    }
}
