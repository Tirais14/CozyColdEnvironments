using CCEnvs.Attributes;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public abstract class ADragHandler<T> : CCBehaviour,
        IBeginDragHandler,
        IDragHandler

        where T : Component, IDraggable
    {
        protected T draggable;

        [RequiredField]
        protected ICanvasRaycaster raycaster;

        protected virtual bool CanBeginDrag => true;

        protected override void OnStart()
        {
            base.OnStart();

            var canvasController = this.GetAssignedObjectInParent<ICanvasController>()
                                       .ThrowIfNull(new ObjectNotFoundException(typeof(ICanvasController)));

            raycaster = canvasController.RaycasterCanvas;

            onEndFirstFrame += () => draggable.gameObject.SetActive(false);

            draggable.OnEndDragEvent += EndDrag;
        }

        protected virtual void BeginDrag(PointerEventData eventData)
        {
            draggable.gameObject.SetActive(true);
            eventData.pointerDrag = draggable.gameObject;
        }

        protected virtual void EndDrag(PointerEventData eventData)
        {
            draggable.gameObject.SetActive(false);

            IDropHandler[] dropHandlers = raycaster.Raycast<IDropHandler>(eventData.position);

            if (dropHandlers.IsEmpty())
                return;

            if (dropHandlers.Length > 1)
                throw new System.Exception("Cannot resolve drop handler to execute.");

            dropHandlers[0].OnDrop(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!CanBeginDrag)
            {
                eventData.pointerDrag = null;
                return;
            }

            BeginDrag(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
        }
    }
}
