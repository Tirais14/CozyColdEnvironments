using UniRx;
using UnityEngine;
using CCEnvs.GameSystems.ItemStorageSystem;
using CCEnvs.UI.Storages;

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
