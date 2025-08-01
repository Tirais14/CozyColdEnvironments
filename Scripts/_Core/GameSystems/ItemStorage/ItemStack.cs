#nullable enable
using System;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public class ItemStack : IItemStack
    {
        public static ItemStack Empty => new();

        public IStorageItem Item { get; private set; } = new StorageItem();
        public int ItemCount { get; private set; }
        public int MaxItemCount { get; private set; }
        public bool IsEmpty => ItemCount < 1 || Item.IsNull();
        public bool IsFull => ItemCount >= MaxItemCount;

        public ItemStack(int maxItemCount = int.MaxValue)
        {
            if (maxItemCount < 1)
                throw new ArgumentException($"{nameof(MaxItemCount)} cannot be {maxItemCount}.");

            MaxItemCount = maxItemCount;
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

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IItemStack AddItem(IStorageItem item, int count)
        {
            if (!IsEmpty && !Item!.Equals(item))
                throw new Exception($"{GetType().GetName()} is not empty and items not equals.");
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            int toAddCount = ItemStackHelper.CalulcateToAddCount(this, count);

            Item = item;
            ItemCount += toAddCount;

            if (toAddCount < count)
                return new ItemStack(item, count - toAddCount);

            return Empty;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItemStack itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (itemStack.IsEmpty)
                return;
            if (ReferenceEquals(itemStack, this))
                throw new InvalidOperationException("Couldn't be added items by itself.");

            IItemStack taked = itemStack.Take(count);

            if (taked.IsEmpty)
                return;

            AddItem(taked.Item!, taked.ItemCount);

            if (!itemStack.IsEmpty)
                itemStack.AddItem(taked.Item!, taked.ItemCount);
        }

        /// <exception cref="ArgumentException"></exception>
        public IItemStack Take(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (IsEmpty)
                return Empty;

            count = ItemStackHelper.CalculateToTakeCount(this, count);

            if (count < 1)
                return Empty;

            ItemCount -= count;

            var taked = new ItemStack(Item, count, count);

            if (ItemCount <= 0)
                Clear();

            return taked;
        }

        public IItemStack TakeAll()
        {
            if (IsEmpty)
                return Empty;

            return Take(ItemCount);
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
        public bool IsEmpty => stack.IsEmpty;
        public bool IsFull => stack.IsFull;

        public ItemStack(int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack(maxItemCount);
        }

        public ItemStack(T item, int itemCount, int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack(item, itemCount, maxItemCount);
        }

        public IItemStack<T> AddItem(T item, int count)
        {
            IItemStack nonTyped = stack.AddItem(item, count);

            return new ItemStack<T>((T)nonTyped.Item, nonTyped.ItemCount);
        }

        public void AddItem(IItemStack<T> itemStack, int count)
        {
            stack.AddItem(itemStack, count);
        }

        public void Clear() => stack.Clear();

        public IItemStack<T> Take(int count)
        {
            IItemStack nonTyped = stack.Take(count);

            return new ItemStack<T>((T)nonTyped.Item, nonTyped.ItemCount);
        }

        public virtual IItemStack<T> TakeAll()
        {
            return Take(ItemCount);
        }
    }
}
