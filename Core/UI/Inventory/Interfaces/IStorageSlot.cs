#nullable enable

namespace UTIRLib.UI
{
    public interface IStorageSlot : IReadOnlyStorageSlot, IItemStackProvider
    {
        IItemStack Put(IItem item, int quantity = 1);

        void Put(IItemStack itemStack);

        IItemStack Take(int quantity = 1);

        IItemStack TakeAll();

        void Clear();
    }
}