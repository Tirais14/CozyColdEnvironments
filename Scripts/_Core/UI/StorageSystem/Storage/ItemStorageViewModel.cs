#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageViewModel<T> : ViewModel<T>
        where T : ItemStorageUI
    {
        public ItemStorageViewModel(T model) : base(model)
        {
        }
    }
    public class ItemStorageViewModel : ItemStorageViewModel<ItemStorageUI>
    {
        public ItemStorageViewModel(ItemStorageUI model) : base(model)
        {
        }
    }
}
