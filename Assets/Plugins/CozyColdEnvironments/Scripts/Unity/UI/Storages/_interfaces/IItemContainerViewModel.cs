#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerViewModel<out T> : IViewModel<T>
        where T : IItemContainerInfo
    {
        IReadOnlyReactiveProperty<Sprite> ItemIconView { get; }
        IReadOnlyReactiveProperty<string> ItemCountView { get; }
    }
}
