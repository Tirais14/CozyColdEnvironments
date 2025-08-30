using CCEnvs.Unity.UI.MVVM;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemStorageViewModel<T>
        
        : AViewModel<T>,
        IItemStorageViewModel<T>

        where T : IItemStorageReactive
    {
        public IReadOnlyReactiveProperty<bool> IsOpenedView => model.IsOpenedReactive;

        public ItemStorageViewModel(T model) : base(model)
        {
        }
    }
}
