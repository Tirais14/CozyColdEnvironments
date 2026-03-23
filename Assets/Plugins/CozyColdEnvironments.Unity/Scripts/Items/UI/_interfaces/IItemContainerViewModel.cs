#nullable enable
using CCEnvs.Unity.UI;
using R3;
using UnityEngine;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainerViewModel
        :
        IViewModel
    {
        ReadOnlyReactiveProperty<Sprite> Icon { get; }
        ReadOnlyReactiveProperty<string> CounterView { get; }

        CompareAction<int>? ShowCounterTextPredicate { get; set; }
    }
}
