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
    public class ViewElement 
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
                          .Access()
                          .ValidateGetOperation()
                );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(UnityDependecyID.PointerInput)
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
            /// <summary>
            /// Creates a copy of this and do place it in empty space of moved drag item
            /// </summary>
            RefillEmptySpace = 2,
            SetAsLastSiblingWhenDragging = 4,
            /// <summary>
            /// Due dragging do placing this to special <see cref="Canvas"/> marked with <see cref="HighPriorityCanvasMarker"/>. If it doesn't exists prints error.
            /// </summary>
            InHighPriorityCanvas = 8,
        }

        protected DragAndDropSettings dragSettings;
        private Vector2 dragStartPos;
        private int dragStartSiblingIndex;
        private Maybe<ViewElement> dragThisCloned;
        private Maybe<Transform> startDraggingParent;

        protected Lazy<DragAndDropTarget> dragAndDrop { get; private set; } = null!;
        /// <summary>
        /// It's cached result of the <see cref="DragPredicate(out Maybe{string})"/>
        /// </summary>
        protected bool dragAllowed { get; private set; }
        protected Lazy<Maybe<Canvas>> highPriorityCanvas = new(() => new Catched<Canvas>(() => DependencyContainer.Resolve<Canvas>(UnityDependecyID.HighPriorityCanvas)));

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
                dragAllowed = false;
                return;
            }

            dragAllowed = true;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.ResetPos))
                dragStartPos = cTransform.Value.position;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.RefillEmptySpace))
            {
                Hide(); //Hides this before instantiate to avoid graphical glitch
                dragThisCloned = Instantiate(this, cTransform.Value.parent);
                dragThisCloned.IfSome(x =>
                {
                    x.transform.position = cTransform.Value.position;
                    x.Hide();
                });
                Show();
            }

            //Save sibling index before is moved to high priority canvas
            //to correctly restore it position
            if (dragSettings.IsFlagSetted(DragAndDropSettings.SetAsLastSiblingWhenDragging))
                dragStartSiblingIndex = transform.GetSiblingIndex();

            if (dragSettings.IsFlagSetted(DragAndDropSettings.InHighPriorityCanvas))
            {
                highPriorityCanvas.Value.IfSome(x =>
                {
                    startDraggingParent = cTransform.Value.parent;
                    cTransform.Value.parent = x.transform;
                });
            }

            //Set sibling index after moving to high priority canvas for correctly overlapping
            if (dragSettings.IsFlagSetted(DragAndDropSettings.SetAsLastSiblingWhenDragging))
                cTransform.Value.SetAsLastSibling();
        }

        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!dragAllowed)
                return;

            cTransform.Value.position = eventData.position;
        }

        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!dragAllowed)
                return;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.InHighPriorityCanvas))
            {
                highPriorityCanvas.Value.IfSome(x =>
                {
                    startDraggingParent.Match(
                        some: x => cTransform.Value.parent = x,
                        none: () => this.PrintError("Cannot find previous parent transfrom.")
                        );
                });
            }

            if (dragSettings.IsFlagSetted(DragAndDropSettings.ResetPos))
                cTransform.Value.position = dragStartPos;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.RefillEmptySpace))
                dragThisCloned.IfSome(static x => Destroy(x.gameObject));

            if (dragSettings.IsFlagSetted(DragAndDropSettings.SetAsLastSiblingWhenDragging))
                cTransform.Value.SetSiblingIndex(dragStartSiblingIndex);

            dragAllowed = default;
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

        private Subject<Unit>? showableShowSubj;
        private Subject<Unit>? showableHideSubj;
        private ArraySegment<BeforeDisabledGraphicComponentSnapshot> showableDisabledGraphics;
        private bool? isVisible;

        public bool IsVisible => isVisible.GetValueOrDefault();
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
            if (isVisible is not null && !IsVisible)
                return;

            showableDisabledGraphics = UIHelper.DisableGraphics(this);
        }

        public virtual void Show()
        {
            if (IsVisible)
                return;

            UIHelper.EnableGraphics(showableDisabledGraphics);
            isVisible = true;
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
            showableShowSubj ??= new Subject<Unit>();

            return showableShowSubj;
        }

        public IObservable<Unit> ObserveHide()
        {
            showableHideSubj ??= new Subject<Unit>();

            return showableHideSubj;
        }

        #endregion IShowable
    }
}
