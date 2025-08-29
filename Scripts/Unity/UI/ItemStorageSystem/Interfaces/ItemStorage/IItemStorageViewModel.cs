using UniRx;
using CozyColdEnvironments.UI.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface IItemStorageViewModel<T> : IViewModel<T>
        where T : IItemStorageReactive
    {
        IReadOnlyReactiveProperty<bool> IsOpenedView { get; }
    }
}
