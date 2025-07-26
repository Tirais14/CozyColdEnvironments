using UniRx;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStorageViewModel : AViewModel<ItemStorageReactive>
    {
        public IReadOnlyReactiveProperty<bool> IsOpenedView => model.IsOpenedReactive;

        public ItemStorageViewModel(ItemStorageReactive model) : base(model)
        {
        }
    }
}
