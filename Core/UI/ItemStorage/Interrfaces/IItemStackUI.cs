#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStackUI
    {
        IItemUI Item { get; }
        int ItemCount { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        void AddItem(IItemUI item, int count);

        void MoveFrom(IItemStackUI itemStack, int count);

        IItemStackUI Take(int count);

        IItemStackUI TakeAll();
    }
}
