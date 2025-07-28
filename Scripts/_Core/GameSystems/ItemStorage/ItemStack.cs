#nullable enable
using System;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

namespace UTIRLib.GameSystems.Storage
{
    public class ItemStack : IItemStack
    {
        public static ItemStack Empty => new(1);

        public IItem Item { get; private set; } = new NullItem();
        public int ItemCount { get; private set; }
        public int MaxItemCount { get; private set; }
        public bool IsEmpty => ItemCount < 1 || Item is NullItem;
        public bool IsFull => ItemCount >= MaxItemCount;

        public ItemStack(int maxItemCount = int.MaxValue) 
        {
            if (maxItemCount < 1)
                throw new ArgumentException($"{nameof(MaxItemCount)} cannot be {maxItemCount}.");

            MaxItemCount = maxItemCount;
        }

        public ItemStack(IItem item,
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
        public virtual void AddItem(IItem item, int count)
        {
            if (!IsEmpty && !Item.Equals(item))
                throw new Exception($"{GetType().GetName()} is not empty.");
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            count = ItemStackHelper.CalulcateToAddCount(this, count);

            Item = item;
            ItemCount += count;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual void AddItem(IItemStack itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (itemStack.IsEmpty)
                return;

            IItemStack taked = itemStack.Take(count);

            if (taked.IsEmpty)
                return;

            AddItem(taked.Item, taked.ItemCount);

            if (!itemStack.IsEmpty)
                itemStack.AddItem(taked.Item, taked.ItemCount);
        }

        /// <exception cref="ArgumentException"></exception>
        public virtual IItemStack Take(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (IsEmpty)
                return Empty;

            count = ItemStackHelper.CalculateToTakeCount(this, count);

            if (count < 1)
                return Empty;

            ItemCount -= count;

            return new ItemStack(Item, count, count);
        }

        public virtual IItemStack TakeAll()
        {
            if (IsEmpty)
                return Empty;

            return Take(ItemCount);
        }

        public void Clear()
        {
            Item = new NullItem();
            ItemCount = 0;
        }
    }
}
