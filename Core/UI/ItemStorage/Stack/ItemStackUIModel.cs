#nullable enable
using UniRx;

namespace UTIRLib.UI
{
    public class ItemStackUIModel : MonoX, IItemStackUI
    {
        private readonly ItemStackUI stack = new();

        private readonly ReactiveProperty<IItemUI> itemProp = new();
        private readonly ReactiveProperty<int> itemCountProp = new();

        public IItemUI Item => ((IItemStackUI)stack).Item;
        public int ItemCount => ((IItemStackUI)stack).ItemCount;
        public bool IsEmpty => ((IItemStackUI)stack).IsEmpty;
        public bool IsFull => ((IItemStackUI)stack).IsFull;

        public IReadOnlyReactiveProperty<IItemUI> ItemProp => itemProp;
        public IReadOnlyReactiveProperty<int> ItemCountProp => itemCountProp;

        public void AddItem(IItemUI item, int count)
        {
            stack.AddItem(item, count);
        }

        public void MoveFrom(IItemStackUI itemStack, int count)
        {
            stack.MoveFrom(itemStack, count);
        }

        public IItemStackUI Take(int count)
        {
            return stack.Take(count);
        }

        public IItemStackUI TakeAll()
        {
            return stack.TakeAll();
        }
    }
}
