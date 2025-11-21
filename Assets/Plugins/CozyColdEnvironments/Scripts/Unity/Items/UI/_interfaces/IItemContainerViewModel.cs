#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainerViewModel<T> : IViewModel<T>
        where T : IItemContainerInfo
    {
        IReadOnlyReactiveProperty<Sprite> ItemView { get; }
        IReadOnlyReactiveProperty<string> CounterText { get; }
        Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }
    }
}
