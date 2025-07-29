using UniRx;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStorageViewModel<T> : AViewModel<T>, IItemStorageViewModel<T>
        where T : IItemStorageReactive
    {
        public IReadOnlyReactiveProperty<bool> IsOpenedView => model.IsOpenedReactive;

        public ItemStorageViewModel(T model) : base(model)
        {
        }
    }
}
