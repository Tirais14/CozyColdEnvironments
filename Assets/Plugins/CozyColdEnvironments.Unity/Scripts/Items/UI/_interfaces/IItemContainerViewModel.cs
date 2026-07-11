#nullable enable
using CCEnvs.UnityX.UI;
using R3;
using UnityEngine;

namespace CCEnvs.UnityX.Items
{
    public interface IItemContainerViewModel
        :
        IViewModel
    {
        ReadOnlyReactiveProperty<Sprite> Icon { get; }

        ReadOnlyReactiveProperty<string> Count { get; }

        CompareAction<int>? ShowCounterTextPredicate { get; set; }
    }
}
