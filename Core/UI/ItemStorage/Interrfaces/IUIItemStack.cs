#nullable enable
namespace UTIRLib.UI
{
    public interface IUIItemStack
    {
        IUIItem Item { get; }
        int ItemCount { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        void Add(IUIItem item, int count);

        void PutFrom(IUIItemStack itemStack, int count);

        IUIItemStack Take(int count);

        IUIItemStack TakeAll();
    }
}
