#nullable enable
namespace UTIRLib.UI
{
    public interface IUIItemStorage
    {
        int SlotCount { get; }
        IUIItemStack this[int index] { get; }

        void GetItemStack(int index);
    }
}
