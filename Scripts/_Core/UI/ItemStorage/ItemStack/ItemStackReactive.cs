using UniRx;
using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackReactive<T> : ItemStack<T>, IItemStackReactive
        where T : IItem
    {
        private readonly ReactiveProperty<IItem> itemReactive = new(new NullItem());
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public IReadOnlyReactiveProperty<IItem> ItemReactive => itemReactive;
        public IReadOnlyReactiveProperty<int> ItemCountReactive => itemCountReactive;

        public ItemStackReactive(int maxItemCount = int.MaxValue) : base(maxItemCount)
        {
        }

        public ItemStackReactive(T item,
                                 int itemCount = 1,
                                 int maxItemCount = int.MaxValue)
            :
            base(item,
                 itemCount,
                 maxItemCount)
        {
        }

        public override IItemStack<T> AddItem(T item, int count)
        {
            IItemStack<T> rest = base.AddItem(item, count);

            UpdateInfo();

            return rest;
        }

        public override void AddItem(IItemStack<T> itemStack, int count)
        {
            base.AddItem(itemStack, count);

            UpdateInfo();
        }

        public override IItemStack<T> Take(int count)
        {
            IItemStack<T> temp = base.Take(count);
            UpdateInfo();

            return temp;
        }

        public override IItemStack<T> TakeAll()
        {
            IItemStack<T> temp = base.TakeAll();
            UpdateInfo();

            return temp;
        }

        private void UpdateInfo()
        {
            itemReactive.Value = Item;
            itemCountReactive.Value = ItemCount;
        }
    }
    public class ItemStackReactive : ItemStackReactive<IItem> 
    {
        public ItemStackReactive(int maxItemCount = int.MaxValue) : base(maxItemCount)
        {
        }

        public ItemStackReactive(IItem item,
                                 int itemCount = 1,
                                 int maxItemCount = int.MaxValue)
            :
            base(item,
                 itemCount,
                 maxItemCount)
        {
        }
    }
}
