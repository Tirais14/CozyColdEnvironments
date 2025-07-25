using UTIRLib.Patterns.Factory;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageFactory : IFactory<IItemSlotUI[], IItemStorageUI>
    {
        public IItemStorageUI Create(IItemSlotUI[] slots)
        {
            return new ItemStorageUI(slots);
        }
    }
}
