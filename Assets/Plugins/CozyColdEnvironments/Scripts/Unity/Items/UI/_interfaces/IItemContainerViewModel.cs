#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainerViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<Sprite> ItemView { get; }
        IReadOnlyReactiveProperty<string> CounterText { get; }
        Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }
    }
}
