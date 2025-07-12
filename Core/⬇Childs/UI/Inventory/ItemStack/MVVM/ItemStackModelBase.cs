using System.Diagnostics.CodeAnalysis;
using UniRx;

#nullable enable

namespace UTIRLib.UI
{
    public abstract class ItemStackModelBase : MonoX, IItemStack
    {
        protected readonly ReactiveProperty<IItem?> itemProp = new();
        protected readonly ReactiveProperty<int> quantityProp = new();

        public IReadOnlyReactiveProperty<IItem?> ItemProp => itemProp;
        public IReadOnlyReactiveProperty<int> QuantityProp => quantityProp;
        public abstract bool IsEmpty { get; }
        public abstract bool IsFull { get; }
        public abstract IItem? Item { get; }
        public abstract int Quantity { get; }

        public abstract T? GetItem<T>() where T : IItem;

        public abstract bool TryGetItem<T>([NotNullWhen(true)] out T? item) where T : IItem;

        public abstract IItemStack Put(IItem item, int quantity = 1);

        public abstract void Put(IItemStack itemStack);

        public abstract IItemStack Take(int quantity = 1);

        public abstract IItemStack TakeAll();

        public abstract bool Contains(IItem item);

        public abstract void Clear();

        protected void UpdateInfo()
        {
            UpdateReactiveItem();
            UpdateReactiveCount();
        }

        protected abstract void UpdateReactiveItem();

        protected abstract void UpdateReactiveCount();
    }
}