#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.Language;
using CCEnvs.Unity.Dependencies;
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
    public class Element 
        : Showable,
        IElement,
        IDragAndDropToggle
    {
        protected Maybe<Image> image { get; private set; }
        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<DragAndDropToggle> dragAndDropToggle { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;
        protected virtual Maybe<Component> dragItem => this;
        protected virtual bool readyToDrag => true;
        protected virtual bool readyToTakeDrop => false;

        int IDragAndDropToggle.BindingCount => dragAndDropToggle.Value.BindingCount;

        protected override void Awake()
        {
            base.Awake();

            image = GetComponent<Image>();

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
        }

        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!readyToDrag)
                return;

            cTransform.Value.position = pointerInput.Value.InputValue;
        }

        protected virtual void OnEndDrag(PointerEventData eventData)
        {
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
