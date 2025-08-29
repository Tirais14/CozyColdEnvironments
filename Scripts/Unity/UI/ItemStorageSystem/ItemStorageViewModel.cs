using UniRx;
using CozyColdEnvironments.UI.MVVM;

#nullable enable
namespace CozyColdEnvironments.UI.ItemStorageSystem
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
