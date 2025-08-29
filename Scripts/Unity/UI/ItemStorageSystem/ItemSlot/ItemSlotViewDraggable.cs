using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CCEnvs.GameSystems.ItemStorageSystem;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemSlotViewDraggable<TViewModel, TModel> 
        :
        ItemSlotView<TViewModel, TModel>,
        IDraggable

        where TViewModel : IItemSlotViewModel<TModel>
        where TModel : IItemSlot, IItemContainerReactive
    {
        new protected Transform transform;

        private Action<PointerEventData>? onEndDragEvent;

        public event Action<PointerEventData> OnEndDragEvent {
            add
            {
                if (onEndDragEvent.IsInInvocationList(value))
                    return;

                onEndDragEvent += value;
            }
            remove => onEndDragEvent -= value;
        }

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
            if (onEndDragEvent is null)
                throw new NullReferenceException($"{nameof(OnEndDragEvent)} not setted.");

            Debug.Log("Dragged");
            onEndDragEvent(eventData);
        }
    }
}
