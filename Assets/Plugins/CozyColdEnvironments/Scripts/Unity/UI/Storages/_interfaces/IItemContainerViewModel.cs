#nullable enable
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.Storages
{
    public interface IItemContainerViewModel<T> : IViewModel<T>
        where T : IItemContainerInfo
    {
        IReadOnlyReactiveProperty<Sprite?> ItemIcon { get; }
        IReadOnlyReactiveProperty<bool> ItemIconVisible { get; }
        IReadOnlyReactiveProperty<string> ItemCount { get; }
        IReadOnlyReactiveProperty<bool> ItemCountVisible { get; }
        IReadOnlyReactiveProperty<bool> IsActiveContainer { get; }
    }
}
