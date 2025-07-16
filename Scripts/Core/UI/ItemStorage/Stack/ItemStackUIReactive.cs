using UniRx;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemStackUIReactive : IItemStackUI, IItemStackUIReactive
    {
        private readonly ItemStackUI itemStack = new();
        private readonly ReactiveProperty<IItemUI> item = new(new NullItemUI());
        private readonly ReactiveProperty<int> itemCount = new();

        public IReadOnlyReactiveProperty<IItemUI> Item => item;
        public IReadOnlyReactiveProperty<int> ItemCount => itemCount;
        public bool IsEmpty => itemStack.IsEmpty;
        public bool IsFull => itemStack.IsFull;

        IItemUI IItemStackUI.Item => item.Value;
        int IItemStackUI.ItemCount => itemCount.Value;

        public void AddItem(IItemUI item, int count)
        {
            itemStack.AddItem(item, count);
        }

        public void MoveFrom(IItemStackUI from, int count)
        {
            itemStack.MoveFrom(from, count);
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
