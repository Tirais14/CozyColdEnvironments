#nullable enable
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerViewModel
    {
        IReadOnlyReactiveProperty<Sprite> ItemIconView { get; }
        IReadOnlyReactiveProperty<int> ItemCountView { get; }
    }
}
