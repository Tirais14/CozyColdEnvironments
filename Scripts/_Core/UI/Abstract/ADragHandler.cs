using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.InputSystem;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib.UI
{
    public abstract class ADragHandler<T> : MonoX,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
        where T : UnityEngine.Component
    {
        protected T dragItem;
        protected Transform dragItemTransform;
        protected Vector3 beforeDragPosition;
        protected ICanvasRaycaster raycaster;
        protected IPointerInput pointerInput;

        protected abstract bool isDragging { get; set; }

        protected override void OnStart()
        {
            base.OnStart();

            dragItemTransform = dragItem.transform;

            var canvasController = GetComponentInParent<ACanvasController>();
            raycaster = canvasController.RaycasterCanvas;
            pointerInput = canvasController.Pointer;
        }

        public abstract void OnBeginDrag(PointerEventData eventData);

        public abstract void OnDrag(PointerEventData eventData);

        public abstract void OnEndDrag(PointerEventData eventData);

        protected void BeginDrag(PointerEventData eventData)
        {
            isDragging = true;

            beforeDragPosition = dragItemTransform.localPosition;
            eventData.pointerDrag = dragItem.gameObject;
        }

        protected void FollowPointer(PointerEventData eventData)
        {
            dragItemTransform.position = eventData.position;
        }

        protected void EndDrag()
        {
            dragItemTransform.localPosition = beforeDragPosition;
            isDragging = false;
        }
    }
}
