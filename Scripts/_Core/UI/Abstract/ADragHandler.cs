using UnityEngine.EventSystems;

#pragma warning disable IDE0044
#nullable enable
namespace UTIRLib.UI
{
    public abstract class ADragHandler<T> : MonoX,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
        where T : IMovable
    {
        protected T movable;
        protected bool isDragging;

        [GetByParent]
        protected ACanvasController canvasController = null!;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            movable.Position = eventData.pressPosition;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            movable.Position = eventData.position;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (isDragging
                && 
                canvasController.Raycaster.TryRaycastFirst<IDropHandler>(out var dropHandler))
                dropHandler.OnDrop(eventData);

            movable.ResetPosition();
        }
    }
}
