using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.UI.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStackViewModel<T> : IViewModel<T>
        where T : IItemStackReactive
    {
        IReadOnlyReactiveProperty<string> CounterView { get; } 
        IReadOnlyReactiveProperty<Sprite?> IconView { get; }

        void OnViewDrop(PointerEventData eventData);
    }
}
