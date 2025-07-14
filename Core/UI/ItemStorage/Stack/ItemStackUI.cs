#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.UI
{
    public class ItemStackUI : IItemStackUI
    {
        public static ItemStackUI Empty => new();

        public IItemUI Item { get; private set; }
        public int ItemCount { get; private set; }
        public bool IsEmpty => ItemCount < 1;
        public bool IsFull => ItemCount >= Item.MaxStackCount;

        public ItemStackUI() => Item = new ItemUIEmpty();

        public ItemStackUI(IItemUI item, int itemCount = 1)
        {
            Item = item;
            ItemCount = itemCount;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(IItemUI item, int count)
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
        public void PutFrom(IItemStackUI itemStack, int count)
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

            Add(taked.Item, taked.ItemCount);

            if (!itemStack.IsEmpty)
                itemStack.Add(taked.Item, taked.ItemCount);
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

            return new ItemStackUI(Item, count);
        }

        public IItemStackUI TakeAll()
        {
            if (IsEmpty)
                return Empty;

            return Take(ItemCount);
        }
    }
}
