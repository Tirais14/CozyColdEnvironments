#nullable enable
namespace UTIRLib.GameSystems.Storage
{
    public interface IItemStack
    {
        IItem Item { get; }
        int ItemCount { get; }
        int MaxItemCount { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        void AddItem(IItem item, int count);

        void AddItem(IItemStack itemStack, int count);

        IItemStack Take(int count);

        IItemStack TakeAll();

        void Clear();
    }
}
