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
        IReadOnlyReactiveProperty<Maybe<Sprite>> ItemIconView { get; }
        IReadOnlyReactiveProperty<int> ItemCountView { get; }
    }
}
