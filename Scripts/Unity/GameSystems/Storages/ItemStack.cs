#nullable enable
using System;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;

namespace CCEnvs.Unity.GameSystems.Storages
{
    public class ItemStack : IItemStack
    {
        public static ItemStack Empty => new();

        public IStorageItem Item { get; private set; } = new StorageItem();

        public int ItemCount { get; private set; }

        public int MaxItemCount { get; private set; }

        public bool HasItem => Item.IsNotNull() && ItemCount > 0;

        public bool IsContainerFull => ItemCount >= MaxItemCount;

        public ItemStack(int maxItemCount)
        {
            if (maxItemCount < 1)
                throw new ArgumentException($"{nameof(MaxItemCount)} cannot be {maxItemCount}.");

            MaxItemCount = maxItemCount;
        }

        public ItemStack()
            :
            this(int.MaxValue)
        {
        }

        public ItemStack(IStorageItem item,
                         int itemCount = 1,
                         int maxItemCount = int.MaxValue)
            :
            this(Math.Max(itemCount, maxItemCount))
        {
            if (itemCount < 1)
                throw new ArgumentException($"Item count cannot be {itemCount}.");

            Item = item;
            ItemCount = itemCount;
        }

        public ItemStack(ItemStack stack)
        {
            Item = stack.Item;
            ItemCount = stack.ItemCount;
            MaxItemCount = stack.MaxItemCount;
        }

        public ItemStack(IItemStack stack)
            :
            this(stack.Item, stack.ItemCount, stack.MaxItemCount)
        {
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IItemContainer AddItem(IStorageItem item, int count)
        {
            if (!IsSameItem(item))
                throw new Exception($"Is not same item.");
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            int toAddCount = ItemContainerHelper.CalulcateAddItemCount(this, count);

            Item = item;
            ItemCount += toAddCount;

            if (toAddCount < count)
                return new ItemStack(item, count - toAddCount);

            return Empty;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItemFrom(IItemContainer itemContainer, int count)
        {
            if (itemContainer.IsNull())
                throw new ArgumentNullException(nameof(itemContainer));
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (!itemContainer.HasItem)
                return;
            if (ReferenceEquals(itemContainer, this))
                throw new InvalidOperationException("Couldn't be added items by itself.");

            IItemContainer taked = itemContainer.TakeItem(count);

            if (!taked.HasItem)
                return;

            AddItem(taked.Item, taked.ItemCount);
        }
        public void AddItemFrom(IItemContainer itemContainer)
        {
            AddItemFrom(itemContainer, itemContainer.ItemCount);
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

        /// <exception cref="ArgumentException"></exception>
        public IItemContainer TakeItem(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (!HasItem)
                throw new Exception($"{GetType().GetName()} is empty.");

            count = ItemContainerHelper.CalculateTakeItemCount(this, count);

            if (count < 1)
                return Empty;

            ItemCount -= count;

            var taked = new ItemStack(Item, count, count);

            if (ItemCount <= 0)
                Clear();

            return taked;
        }

        public IItemContainer TakeItemAll()
        {
            if (!HasItem)
                return Empty;

            return TakeItem(ItemCount);
        }

        public bool Contains(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            return Item.Equals(item);
        }

        public bool CanHold(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            return true;
        }

        public bool IsSameItem(IStorageItem item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            return !HasItem || HasItem && Item.Equals(item);
        }

        public void Clear()
        {
            Item = new StorageItem();
            ItemCount = 0;
        }
    }
    public class ItemStack<T> : IItemStack<T>
        where T : IStorageItem, new()
    {
        protected readonly ItemStack stack;

        public T Item => (T)stack.Item;
        public int ItemCount => stack.ItemCount;
        public int MaxItemCount => stack.MaxItemCount;
        public bool HasItem => stack.HasItem;
        public bool IsContainerFull => stack.IsContainerFull;

        IStorageItem IItemContainerInfo.Item => Item;

        public ItemStack(int maxItemCount)
        {
            stack = new ItemStack(maxItemCount);
        }

        public ItemStack()
            :
            this(int.MaxValue)
        {
        }

        public ItemStack(T item, int itemCount, int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack(item, itemCount, maxItemCount);
        }

        public ItemStack(ItemStack<T> stack)
            :
            this(stack.Item, stack.ItemCount, stack.MaxItemCount)
        {
        }

        public IItemStack<T> AddItem(T item, int count)
        {
            IItemContainer nonTyped = stack.AddItem(item, count);

            return new ItemStack<T>((T)nonTyped.Item, nonTyped.ItemCount);
        }

        public void AddItemFrom(IItemStack<T> itemStack, int count)
        {
            stack.AddItemFrom(itemStack, count);
        }
        public void AddItemFrom(IItemStack<T> itemStack)
        {
            stack.AddItemFrom(itemStack);
        }

        public void CopyItemFrom(IItemStack<T> itemStack)
        {
            stack.CopyItemFrom(itemStack);
        }

        public IItemStack<T> TakeItem(int count)
        {
            IItemContainer nonTyped = stack.TakeItem(count);

            return new ItemStack<T>((T)nonTyped.Item, nonTyped.ItemCount);
        }

        public virtual IItemStack<T> TakeItemAll() => TakeItem(ItemCount);

        public bool Contains(T item) => stack.Contains(item);

        public bool CanHold(T item) => stack.CanHold(item);

        public bool IsSameItem(T item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            return Item.IsNotNull() && Item.Equals(item);
        }

        public void Clear() => stack.Clear();
    }
}
