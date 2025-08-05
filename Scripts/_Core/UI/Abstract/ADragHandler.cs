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
        [GetBySelf]
        protected T dragItem;

        protected Transform dragItemTransform;
        protected Vector3 beforeDragPosition;
        protected ICanvasRaycaster raycaster;
        protected IPointerInput pointerInput;

        protected abstract bool isDragging { get; set; }

        protected override void OnAwake()
        {
            base.OnAwake();

            dragItemTransform = dragItem.transform;
            var canvasController = GetComponentInParent<ACanvasController>();
            raycaster = canvasController.CanvasRaycaster;
            pointerInput = canvasController.Pointer;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            beforeDragPosition = dragItemTransform.localPosition;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            dragItemTransform.position = eventData.position;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (isDragging)
                return;

            if (raycaster.TryRaycastAny<IDropHandler>(pointerInput.Value, out var dropHandler))
                dropHandler.OnDrop(eventData);

            dragItemTransform.localPosition = beforeDragPosition;
        }
    }
}
