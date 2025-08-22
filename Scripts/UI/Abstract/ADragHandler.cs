using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;
using UTIRLib.InputSystem;
using UTIRLib.Unity.Extensions;

#nullable enable
namespace UTIRLib.UI
{
    public abstract class ADragHandler<T> : MonoX,
        IBeginDragHandler,
        IDragHandler

        where T : Component, IDraggable
    {
        protected T draggable;

        [RequiredField]
        protected ICanvasRaycaster raycaster;

        [RequiredField]
        protected InputAction pointer;

        protected virtual bool CanBeginDrag => true;

        protected override void OnStart()
        {
            base.OnStart();

            var canvasController = this.GetAssignedObjectInParent<ICanvasController>()
                                       .ThrowIfNull(new ObjectNotFoundException(typeof(ICanvasController)));

            raycaster = canvasController.RaycasterCanvas;
            pointer = canvasController.Pointer;

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
