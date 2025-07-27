using UniRx;
using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackReactive : ItemStack, IItemStackReactive
    {
        private readonly ReactiveProperty<IItem> itemReactive = new(new NullItem());
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public IReadOnlyReactiveProperty<IItem> ItemReactive => itemReactive;
        public IReadOnlyReactiveProperty<int> ItemCountReactive => itemCountReactive;

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

        public override void AddItem(IItem item, int count)
        {
            base.AddItem(item, count);

            UpdateInfo();
        }

        public override void AddItem(IItemStack itemStack, int count)
        {
            base.AddItem(itemStack, count);

            UpdateInfo();
        }

        public override IItemStack Take(int count)
        {
            IItemStack temp = base.Take(count);
            UpdateInfo();

            return temp;
        }

        public override IItemStack TakeAll()
        {
            IItemStack temp = base.TakeAll();
            UpdateInfo();

            return temp;
        }

        private void UpdateInfo()
        {
            itemReactive.Value = Item;
            itemCountReactive.Value = ItemCount;
        }
    }
}
