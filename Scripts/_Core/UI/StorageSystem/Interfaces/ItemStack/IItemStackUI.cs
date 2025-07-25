#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public interface IItemStackUI
    {
        IItemUI Item { get; }
        int ItemCount { get; }
        int MaxItemCount { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        void AddItem(IItemUI item, int count);

        void AddItem(IItemStackUI itemStack, int count);

        IItemStackUI Take(int count);

        IItemStackUI TakeAll();
    }
}
