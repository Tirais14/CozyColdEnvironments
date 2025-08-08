using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public class ItemSlotViewDraggable<TViewModel, TModel> 
        :
        ItemSlotView<TViewModel, TModel>,
        IDraggable

        where TViewModel : IItemSlotViewModel<TModel>
        where TModel : IItemSlot, IItemContainerReactive
    {
        new protected Transform transform;

        public event Action<PointerEventData> OnEndDragEvent = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            transform = base.transform;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (OnEndDragEvent is null)
                throw new NullReferenceException($"{nameof(OnEndDragEvent)} not setted.");

            Debug.Log("Dragged");
            OnEndDragEvent(eventData);
        }
    }
}
