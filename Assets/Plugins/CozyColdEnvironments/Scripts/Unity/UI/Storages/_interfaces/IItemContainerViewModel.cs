#nullable enable
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.Storages
{
    public interface IItemContainerViewModel<out T> : IViewModel<T>
        where T : IItemContainerInfo
    {
        IReadOnlyReactiveProperty<Sprite?> ItemIconView { get; }
        IReadOnlyReactiveProperty<bool> ItemIconVisible { get; }
        IReadOnlyReactiveProperty<string> ItemCountView { get; }
        IReadOnlyReactiveProperty<bool> ItemCountVisible { get; }
    }
}
