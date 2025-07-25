#nullable enable
using System;
using System.Collections.Generic;
using UniRx;
using UTIRLib.Diagnostics;
using UTIRLib.UI.StorageSystem;

namespace UTIRLib.UI.StorageSystem
{
    public class ItemStackUI : IItemStackUI
    {
        public static ItemStackUI Empty => new(1);

        public IItemUI Item { get; private set; } = new NullItemUI();
        public int ItemCount { get; private set; }
        public int MaxItemCount { get; private set; }
        public bool IsEmpty => ItemCount < 1;
        public bool IsFull => ItemCount >= MaxItemCount;

        public ItemStackUI(int maxItemCount) 
        {
            if (maxItemCount < 1)
                throw new ArgumentException($"{nameof(MaxItemCount)} cannot be {maxItemCount}.");

            MaxItemCount = maxItemCount;
        }

        public ItemStackUI(int maxItemCount, IItemUI item, int itemCount = 1)
            : 
            this(maxItemCount)
        {
            if (itemCount < 1)
                throw new ArgumentException($"Item count cannot be {itemCount}.");

            Item = item;
            ItemCount = itemCount;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItemUI item, int count)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            count = ItemStackUIHelper.CalulcateToAddCount(this, count);

            ItemCount += count;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddItem(IItemStackUI itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (itemStack.IsEmpty)
                return;

            IItemStackUI taked = itemStack.Take(count);

            if (taked.IsEmpty)
                return;

            AddItem(taked.Item, taked.ItemCount);

            if (!itemStack.IsEmpty)
                itemStack.AddItem(taked.Item, taked.ItemCount);
        }

        /// <exception cref="ArgumentException"></exception>
        public IItemStackUI Take(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (IsEmpty)
                return Empty;

            count = ItemStackUIHelper.CalculateToTakeCount(this, count);

            if (count < 1)
                return Empty;

            ItemCount -= count;

            return new ItemStackUI(count, Item, count);
        }

        public IItemStackUI TakeAll()
        {
            if (IsEmpty)
                return Empty;

            return Take(ItemCount);
        }
    }
}
