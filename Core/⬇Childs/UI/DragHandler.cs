using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.Attributes;
using UTIRLib.Injector;
using UTIRLib.InputSystem;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.UI
{
    public abstract class DragHandler<T> : MonoX,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
        where T : Object
    {
        [RequiredMember]
        protected IPointerHandler pointerHandler = null!;

        [Optional]
        [SerializeField]
        [Tooltip("If empty would be used transform.position")]
        protected Vector2 localDefaultPosition;

        [SerializeField]
        [GetComponentInParentIfNull]
        protected UserInterface userInterface = null!;

        [SerializeField]
        [GetComponentInChildrenIfNull]
        protected T draggable = null!;

#pragma warning disable IDE1006 // Naming Styles

        public event Action<IUserInterface, T>? onBeginDrag;

        /// <summary>Calls before reset position</summary>
        public event Action<IUserInterface, T>? onEndDrag;

        public event Action<IUserInterface, T>? onDrop;

#pragma warning restore IDE1006 // Naming Styles

        protected override void OnAwake()
        {
            base.OnAwake();

            pointerHandler = userInterface.PointerHandler;

            if (localDefaultPosition == default)
            {
                localDefaultPosition = transform.localPosition;
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData) =>
            onBeginDrag?.Invoke(userInterface, draggable);

        public virtual void OnDrag(PointerEventData eventData) =>
            transform.localPosition = pointerHandler.PointerPosition;

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(userInterface, draggable);
            transform.localPosition = localDefaultPosition;
        }

        public virtual void OnDrop(PointerEventData eventData) => onDrop?.Invoke(userInterface, draggable);
    }

    public class DragHandler : DragHandler<Component>
    {
    }
}