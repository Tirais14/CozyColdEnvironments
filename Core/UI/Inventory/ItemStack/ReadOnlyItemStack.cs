using System;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.UI
{
    public class ReadOnlyItemStack : IReadOnlyItemStack
    {
        protected IItem? item;
        protected int quantity;

        public static ReadOnlyEmptyItemStack empty = new();
        public IItem? Item => item;
        public int Quantity => quantity;
        public bool IsEmpty => item == null || quantity == 0;
        public bool IsFull => item != null && quantity >= (item?.MaxQuantity ?? 0);

        public ReadOnlyItemStack()
        {
        }

        public ReadOnlyItemStack(IItem? item, int quantity = 1)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public ReadOnlyItemStack(IItemContainer itemProvider) : this(itemProvider.Item, itemProvider.Quantity)
        {
        }

        public T? GetItem<T>() where T : IItem
        {
            if (item is T typedItem)
            {
                return typedItem;
            }

            return default;
        }

        public bool TryGetItem<T>([NotNullWhen(true)] out T? item) where T : IItem
        {
            item = GetItem<T>();

            return item.IsNotDefault();
        }

        public bool Contains(IItem item) => !IsEmpty && this.item == item;
    }
}