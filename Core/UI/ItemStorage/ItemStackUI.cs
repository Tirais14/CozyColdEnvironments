#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.UI
{
    public class ItemStackUI : IUIItemStack
    {
        public static ItemStackUI Empty => new();

        public IUIItem Item { get; private set; } = new ItemUIEmpty();
        public int ItemCount { get; private set; }
        public bool IsEmpty => ItemCount < 1;
        public bool IsFull => ItemCount >= Item.MaxStackCount;

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(IUIItem item, int count)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            count = UIItemStackHelper.CalulcateToAddCount(this, count);

            ItemCount += count;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void PutFrom(IUIItemStack itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (itemStack.IsEmpty)
                return;

            IUIItemStack taked = itemStack.Take(count);

            if (taked.IsEmpty)
                return;

            Add(taked.Item, taked.ItemCount);

            if (!itemStack.IsEmpty)
                itemStack.Add(taked.Item, taked.ItemCount);
        }

        /// <exception cref="ArgumentException"></exception>
        public IUIItemStack Take(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (IsEmpty)
                return Empty;

            count = UIItemStackHelper.CalculateToTakeCount(this, count);

            if (count < 1)
                return Empty;

            ItemCount -= count;
        }

        public IUIItemStack TakeAll()
        {
            if (IsEmpty)
                return Empty;

            return Take(ItemCount);
        }
    }
}
