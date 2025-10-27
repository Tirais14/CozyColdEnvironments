#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236

namespace CCEnvs.Unity.UI.Elements
{
    [DisallowMultipleComponent]
    public class ViewElement 
        : Showable,
        IViewElement,
        IDragAndDropToggle
    {
        protected Maybe<Component> _dragItem;
        private Vector2 beforeDragPosition;

        [field: GetBySelf]
        public Maybe<Image> image { get; private set; }
        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<DragAndDropToggle> dragAndDropToggle { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;
        protected Component dragItem => _dragItem.Access(this)!;
        protected virtual bool dragCopyOfThis => false;
        protected virtual (bool state, Maybe<string> msg) readyToDrag => (true, null);
        protected virtual bool readyToTakeDrop => false;
        protected virtual bool resetPositionAfterDrag => false;

        int IDragAndDropToggle.BindingCount => dragAndDropToggle.Value.BindingCount;

        protected override void Awake()
        {
            base.Awake();

            canvasController = new Lazy<ICanvasController>(
                () => this.GetAssignedObjectInParent<ICanvasController>(includeInactive: true)
                .ValidateGetOperation()
                );

            dragAndDropToggle = new Lazy<DragAndDropToggle>(
                () => new DragAndDropToggle(
                    gameObject,
                    OnBeginDrag,
                    OnDrag,
                    OnEndDrag,
                    OnDrop)
                );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(DependencyID.PointerInput)
                );
        }

        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            readyToDrag.msg.IfSome(x => this.PrintLog(x));

            if (!readyToDrag.state)
                return;

            if (resetPositionAfterDrag)
                beforeDragPosition = transform.position;

            if (dragCopyOfThis)
            {
                _dragItem = Instantiate(this, transform.parent);
                Hide();
            }
        }

        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!readyToDrag.state)
                return;

            dragItem.transform.position = eventData.position;
        }

        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!readyToDrag.state)
                return;

            if (!dragCopyOfThis && resetPositionAfterDrag)
                transform.position = beforeDragPosition;

            if (dragCopyOfThis)
            {
                _dragItem.Match(
                    some: cmp => Destroy(cmp.gameObject),
                    none: () => this.PrintWarning("Copy of drag item is not destroyed. May cause a memory leak.")
                    );

                Show();
            }
        }

        protected virtual void OnDrop(PointerEventData eventData)
        {
        }

        void IDragAndDropToggle.ActivateDragAndDropAbility()
        {
            dragAndDropToggle.Value.ActivateDragAndDropAbility();
        }

        void IDragAndDropToggle.DeactivateDragAndDropAbility()
        {
            dragAndDropToggle.Value.DeactivateDragAndDropAbility();
        }
    }
}
