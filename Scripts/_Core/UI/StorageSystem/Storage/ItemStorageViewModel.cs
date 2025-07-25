#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageViewModel<T> : ViewModel<T>
        where T : IItemStorageUI
    {
        public ItemStorageViewModel(T model) : base(model)
        {
        }
    }
    public class ItemStorageViewModel : ItemStorageViewModel<IItemStorageUI>
    {
        public ItemStorageViewModel(IItemStorageUI model) : base(model)
        {
        }
    }
}
