using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.UI
{
    public class ItemStackModel<TItemStack> : ItemStackModelBase, IItemStack
        where TItemStack : IItemStack, new()
    {
        protected readonly TItemStack itemStackInternal = new();

        public override IItem? Item => itemStackInternal.Item;
        public override int Quantity => itemStackInternal.Quantity;
        public override bool IsEmpty => itemStackInternal.IsEmpty;
        public override bool IsFull => itemStackInternal.IsFull;

        public override T? GetItem<T>() where T : default => itemStackInternal.GetItem<T>();

        public override bool TryGetItem<T>([NotNullWhen(true)] out T? item) where T : default =>
            itemStackInternal.TryGetItem(out item);

        public override IItemStack Put(IItem item, int quantity = 1)
        {
            IItemStack remainingItems = itemStackInternal.Put(item, quantity);
            UpdateInfo();
            return remainingItems;
        }

        public override void Put(IItemStack itemStack)
        {
            itemStackInternal.Put(itemStack);
            UpdateInfo();
        }

        public override IItemStack Take(int quantity = 1)
        {
            IItemStack remainingItems = itemStackInternal.Take(quantity);
            UpdateInfo();
            return remainingItems;
        }

        public override IItemStack TakeAll() => itemStackInternal.TakeAll();

        public override bool Contains(IItem item) => itemStackInternal.Contains(item);

        public override void Clear() => itemStackInternal.Clear();

        protected override void UpdateReactiveItem() => itemProp.Value = itemStackInternal.Item;

        protected override void UpdateReactiveCount() => quantityProp.Value = itemStackInternal.Quantity;
    }

    public class ItemStackModel : ItemStackModel<ItemStack>
    {
    }
}