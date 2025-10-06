using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.Storages;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.UI
{
    public interface IItemStackViewModel<T> : IViewModel<T>
        where T : IItemStack, IItemContainerReactive
    {
        IReadOnlyReactiveProperty<string> CounterView { get; } 
        IReadOnlyReactiveProperty<Sprite?> IconView { get; }
    }
}
