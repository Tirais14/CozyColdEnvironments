using UniRx;
using UnityEngine;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.UI.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStackViewModel<T> : IViewModel<T>
        where T : IItemStack, IItemContainerReactive
    {
        IReadOnlyReactiveProperty<string> CounterView { get; } 
        IReadOnlyReactiveProperty<Sprite?> IconView { get; }
    }
}
