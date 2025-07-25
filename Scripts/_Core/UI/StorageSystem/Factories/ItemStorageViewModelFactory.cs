#nullable enable
using UTIRLib.Patterns.Factory;

namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageViewModelFactory : 
        IFactory<IItemSlotUI[], IViewModel<IItemStorageUI>>
    {
        public IViewModel<IItemStorageUI> Create(IItemSlotUI[] slots)
        {
            IItemStorageUI storage = new ItemStorageFactory().Create(slots);

            return new ItemStorageViewModel(storage);
        }
    }
}
