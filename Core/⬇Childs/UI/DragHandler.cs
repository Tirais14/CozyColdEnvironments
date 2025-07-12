using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;
using UTIRLib.Injector;
using UTIRLib.InputSystem;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.UI
{
#pragma warning disable IDE1006 // Naming Styles
    public abstract class DragHandler<T> : MonoX,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler
        where T : Object
    {
        [RequiredMember]
        protected IPointerHandler pointerHandler = null!;

        [Optional]
        [SerializeField]
        [Tooltip("If empty would be used transform.position")]
        protected Vector2 localDefaultPosition;

        [SerializeField]
        [GetComponentInChildrenIfNull]
        protected T draggable = null!;

        public event Action<T>? onBeginDrag;
        /// <summary>Calls before reset position</summary>
        public event Action<T>? onEndDrag;
        public event Action<T>? onDrop;

        protected override void OnAwake()
        {
            base.OnAwake();

            if (GetComponentInParent<IUserInterface>().Is<IUserInterface>(out var ui))
                pointerHandler = ui.PointerHandler;
            else
                throw new ObjectNotFoundException(typeof(IUserInterface));

            if (localDefaultPosition == default)
                localDefaultPosition = transform.localPosition;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(draggable);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = pointerHandler.PointerPosition;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(draggable);
            transform.localPosition = localDefaultPosition;
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            onDrop?.Invoke(draggable);
        }
    }
}