using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UTIRLib.UI.ItemStorage;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStackViewModel<T> : IViewModel<T>
        where T : IItemStackReactive
    {
        void OnViewDrop(PointerEventData eventData);
    }
}
