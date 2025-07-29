using UniRx;
using UTIRLib.UI.ItemStorage;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStorageViewModel<T> : IViewModel<T>
        where T : IItemStorageReactive
    {
        IReadOnlyReactiveProperty<bool> IsOpenedView { get; }
    }
}
