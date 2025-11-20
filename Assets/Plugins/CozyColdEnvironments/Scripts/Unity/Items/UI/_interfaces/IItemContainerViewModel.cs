#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainerViewModel<T> : IPresenter<T>
        where T : IItemContainerInfo
    {
        IReadOnlyReactiveProperty<Sprite> ItemIcon { get; }
        IReadOnlyReactiveProperty<string> ItemCount { get; }
        IReadOnlyReactiveProperty<bool> IsActiveContainer { get; }
        Maybe<CompareAction<int>> ShowItemCounterPredicate { get; set; }

        void SetActiveState(Maybe<bool> state = default);
    }
}
