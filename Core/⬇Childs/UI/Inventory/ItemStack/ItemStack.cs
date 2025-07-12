using System;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.UI
{
    public class ItemStack : ReadOnlyItemStack, IItemStack
    {
        public ItemStack() : base()
        {
        }

        public ItemStack(IItem? item, int quantity = 1) : base(item, quantity)
        {
        }

        public ItemStack(IItemContainer itemProvider) : base(itemProvider)
        {
        }

        /// <returns>Remaining items or <see cref="ReadOnlyEmptyItemStack"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IItemStack Put(IItem item, int quantity = 1)
        {
            if (item.IsNull())
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (quantity == 0)
            {
                return empty;
            }

            quantity = Math.Abs(quantity);
            if (IsEmpty)
            {
                return SetItem(item, quantity);
            }
            if (!IsEmpty && Equals(item) && !IsFull)
            {
                return AddItem(quantity);
            }
            else
            {
                return ReplaceItem(item, quantity);
            }
        }

        /// <returns>Remaining items or <see cref="ReadOnlyEmptyItemStack"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Put(IItemStack itemStack)
        {
            if (itemStack.IsNull())
            {
                throw new ArgumentNullException(nameof(itemStack));
            }
            if (itemStack.IsEmpty)
            {
                return;
            }

            int movedItemCount = itemStack.Quantity - Put(itemStack.Item!, itemStack.Quantity).Quantity;
            if (movedItemCount > 0)
            {
                itemStack.Take(movedItemCount);
            }
        }

        /// <returns>Item stack or <see langword="null"/></returns>
        public virtual IItemStack Take(int quantity = 1)
        {
            if (IsEmpty || quantity == 0)
            {
                return empty;
            }

            quantity = Math.Abs(quantity);
            int takedQuantity = ProccessToTakeQuantity(quantity);
            this.quantity -= takedQuantity;
            IItemStack takedItemStack = takedQuantity > 0 ? new ItemStack(item, takedQuantity) : empty;
            UpdateInfo();
            return takedItemStack;
        }

        public virtual IItemStack TakeAll() => Take(quantity);

        public void Clear()
        {
            item = null;
            quantity = 0;
        }

        protected IItemStack SetItem(IItem item, int quantity)
        {
            this.item = item;
            IItemStack remainingItemStack = CalculateRemainingItemStack(quantity);
            this.quantity = remainingItemStack.IsEmpty ? quantity : item.MaxQuantity;

            return remainingItemStack;
        }

        protected IItemStack AddItem(int quantity)
        {
            IItemStack remainingItemStack = CalculateRemainingItemStack(quantity);
            this.quantity = remainingItemStack.IsEmpty ? this.quantity + quantity : item!.MaxQuantity;

            return remainingItemStack;
        }

        protected IItemStack ReplaceItem(IItem item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;

            return Take(this.quantity);
        }

        /// <returns>Actual taked item quantity</returns>
        protected int ProccessToTakeQuantity(int toTakeQuantity)
        {
            int remainingQuantity = quantity - toTakeQuantity;
            if (remainingQuantity < 0)
            {
                return toTakeQuantity - Math.Abs(remainingQuantity);
            }
            else return toTakeQuantity;
        }

        protected IItemStack CalculateRemainingItemStack(int quantity)
        {
            int totalQuantity = quantity + this.quantity;
            return totalQuantity - item!.MaxQuantity > 0 ? new ItemStack(item, totalQuantity - item.MaxQuantity) : empty;
        }

        protected void UpdateInfo()
        {
            if (quantity <= 0 || item == null)
            {
                Clear();
            }
        }
    }
}