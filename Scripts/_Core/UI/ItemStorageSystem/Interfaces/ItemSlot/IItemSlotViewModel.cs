using UnityEngine.EventSystems;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemSlotViewModel : IViewModel
    {
        void OnViewDrop(PointerEventData eventData);
    }
    public interface IItemSlotViewModel<T> : IItemSlotViewModel, IViewModel<T>
        where T : IItemSlot
    {
    }
}
