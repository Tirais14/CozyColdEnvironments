#nullable enable
using System;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

namespace UTIRLib.GameSystems.Storage
{
    public class ItemStack<T> : IItemStack<T>
        where T : IItem
    {
        public static ItemStack<T> Empty => new(1);

        public T Item { get; private set; } = InstanceFactory.Create<T>(InvokableArguments.Create(new NullItem(), InvokableArguments.CreationSettings.AllowSignatureTypesInheritance), cacheConstructor: true);
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

        public ItemStack(T item,
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
        public virtual IItemStack<T> AddItem(T item, int count)
        {
            if (!IsEmpty && !Item.Equals(item))
                throw new Exception($"{GetType().GetName()} is not empty and items not equals.");
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));
            if (count < 1)
                throw new ArgumentException(nameof(count));

            int toAddCount = ItemStackHelper.CalulcateToAddCount(this, count);

            Item = item;
            ItemCount += toAddCount;

            if (toAddCount < count)
                return new ItemStack<T>(item, count - toAddCount);

            return Empty;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual void AddItem(IItemStack<T> itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new ArgumentNullException(nameof(itemStack));
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (itemStack.IsEmpty)
                return;
            if (ReferenceEquals(itemStack, this))
                throw new InvalidOperationException("Couldn't be added items by itself.");

            IItemStack<T> taked = itemStack.Take(count);

            if (taked.IsEmpty)
                return;

            AddItem(taked.Item, taked.ItemCount);

            if (!itemStack.IsEmpty)
                itemStack.AddItem(taked.Item, taked.ItemCount);
        }

        /// <exception cref="ArgumentException"></exception>
        public virtual IItemStack<T> Take(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));
            if (IsEmpty)
                return Empty;

            count = ItemStackHelper.CalculateToTakeCount(this, count);

            if (count < 1)
                return Empty;

            ItemCount -= count;

            var taked = new ItemStack<T>(Item, count, count);

            if (ItemCount <= 0)
                Clear();

            return taked;
        }

        public virtual IItemStack<T> TakeAll()
        {
            if (IsEmpty)
                return Empty;

            return Take(ItemCount);
        }

        public void Clear()
        {
            Item = InstanceFactory.Create<T>(InvokableArguments.Create(new NullItem(), InvokableArguments.CreationSettings.AllowSignatureTypesInheritance), cacheConstructor: true);
            ItemCount = 0;
        }
    }
    public class ItemStack : ItemStack<IItem>
    {
        public ItemStack(int maxItemCount = int.MaxValue)
            :
            base(maxItemCount)
        {
        }

        public ItemStack(IItem item,
                         int itemCount = 1,
                         int maxItemCount = int.MaxValue)
            :
            base(item, itemCount, maxItemCount)
        {
        }
    }
}
