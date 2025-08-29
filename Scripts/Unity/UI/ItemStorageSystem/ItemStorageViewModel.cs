using UniRx;
using CCEnvs.UI.MVVM;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
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
