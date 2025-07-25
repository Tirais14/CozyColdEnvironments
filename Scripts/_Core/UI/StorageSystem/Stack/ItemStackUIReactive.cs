using UniRx;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemStackUIReactive : IItemStackUIReactive
    {
        private readonly ItemStackUI itemStack;
        private readonly ReactiveProperty<IItemUI> item = new(new NullItemUI());
        private readonly ReactiveProperty<int> itemCount = new();

        public IReadOnlyReactiveProperty<IItemUI> Item => item;
        public IReadOnlyReactiveProperty<int> ItemCount => itemCount;
        public int MaxItemCount => itemStack.MaxItemCount;
        public bool IsEmpty => itemStack.IsEmpty;
        public bool IsFull => itemStack.IsFull;

        IItemUI IItemStackUI.Item => item.Value;
        int IItemStackUI.ItemCount => itemCount.Value;

        public ItemStackUIReactive(int maxItemCount)
        {
            itemStack = new ItemStackUI(maxItemCount);
        }

        public ItemStackUIReactive(int maxItemCount, IItemUI item, int count) 
        {
            itemStack = new ItemStackUI(maxItemCount, item, count);
        }

        public void AddItem(IItemUI item, int count)
        {
            itemStack.AddItem(item, count);
        }

        public void AddItem(IItemStackUI itemStack, int count)
        {
            this.itemStack.AddItem(itemStack, count);
        }

        public IItemStackUI Take(int count)
        {
            return itemStack.Take(count);
        }

        public IItemStackUI TakeAll()
        {
            return itemStack.TakeAll();
        }
    }
}
