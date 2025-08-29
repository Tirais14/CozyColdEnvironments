using UniRx;
using CCEnvs.UI.Storages;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public interface IItemStorageViewModel<T> : IViewModel<T>
        where T : IItemStorageReactive
    {
        IReadOnlyReactiveProperty<bool> IsOpenedView { get; }
    }
}
