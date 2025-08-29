using UniRx;
using UnityEngine;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;
using CozyColdEnvironments.UI.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface IItemStackViewModel<T> : IViewModel<T>
        where T : IItemStack, IItemContainerReactive
    {
        IReadOnlyReactiveProperty<string> CounterView { get; } 
        IReadOnlyReactiveProperty<Sprite?> IconView { get; }
    }
}
