#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236

namespace CCEnvs.Unity.UI.Elements
{
    [DisallowMultipleComponent]
    public partial class ViewElement 
        : CCBehaviour,
        IViewElement,
        IDragAndDropTarget
    {
        [field: GetBySelf]
        public Maybe<Image> Img { get; private set; }

        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();

            canvasController = new Lazy<ICanvasController>(
                () => this.GetAssignedObjectInParent<ICanvasController>(includeInactive: true)
                          .ValidateGetOperation()
                );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(DependencyID.PointerInput)
                );

            AwakeDragAndDrop();
        }

        protected override void Start()
        {
            base.Start();

            StartIShowable();
        }

        protected virtual void OnDestroy()
        {
        }

        #region IDragAndDropTarget

        [Flags]
        protected enum DragAndDropSettings
        {
            None,
            ResetPos,
        }

        protected DragAndDropSettings dragSettings;
        private Vector2 beforeDragPosition;

        protected Lazy<DragAndDropTarget> dragAndDrop { get; private set; } = null!;
        protected bool isDragging { get; private set; }

        int IDragAndDropTarget.BindingCount => dragAndDrop.Value.BindingCount;

        protected virtual bool DragPredicate(out Maybe<string> msg)
        {
            msg = null;
            return true;
        }

        protected virtual bool DropPredicate() => true;

        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!DragPredicate(out Maybe<string> msg))
            {
                msg.IfSome(x => this.PrintLog(x));
                isDragging = false;
                return;
            }

            isDragging = true;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.ResetPos))
                beforeDragPosition = transform.position;
        }

        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            cTransform.Value.position = eventData.position;
        }

        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.ResetPos))
                transform.position = beforeDragPosition;
        }

        protected virtual void OnDrop(PointerEventData eventData)
        {
        }

        private void AwakeDragAndDrop()
        {
            dragAndDrop = new Lazy<DragAndDropTarget>(
                () => new DragAndDropTarget(
                    gameObject,
                    OnBeginDrag,
                    OnDrag,
                    OnEndDrag,
                    OnDrop)
                );
        }

        void IDragAndDropTarget.ActivateDragAndDropAbility()
        {
            dragAndDrop.Value.ActivateDragAndDropAbility();
        }

        void IDragAndDropTarget.DeactivateDragAndDropAbility()
        {
            dragAndDrop.Value.DeactivateDragAndDropAbility();
        }

        #endregion IDragAndDropTarget

        #region IShowable

        private Subject<Unit>? showSubj;
        private Subject<Unit>? hideSubj;

        public bool IsVisible => gameObject.activeSelf;
        protected virtual bool showOnStart { get; }

        private void StartIShowable()
        {
            if (showOnStart)
                Show();
            else
                Hide();
        }

        public virtual void Hide()
        {
            Showable.Hide(this);
        }

        public virtual void Show()
        {
            Showable.Show(this);
        }

        public bool SwitchVisibleState()
        {
            if (IsVisible)
                Hide();
            else
                Show();

            return IsVisible;
        }

        public IObservable<Unit> ObserveShow()
        {
            showSubj ??= new Subject<Unit>();

            return showSubj;
        }

        public IObservable<Unit> ObserveHide()
        {
            hideSubj ??= new Subject<Unit>();

            return hideSubj;
        }

        #endregion IShowable
    }
}
