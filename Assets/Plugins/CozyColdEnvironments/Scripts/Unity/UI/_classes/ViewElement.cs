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

#pragma warning disable S4144
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
                () => this.FindFor()
                          .InParent()
                          .IncludeInactive()
                          .ComponentStrict<ICanvasController>()
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
            StartISelectable();
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
        private bool? dragAllowed;

        public bool DragAllowed {
            get => dragAllowed ?? DragPredicate(out _);
        }
        protected Lazy<DragAndDropTarget> dragAndDrop { get; private set; } = null!;
        /// <summary>
        /// It's cached result of the <see cref="DragPredicate(out Maybe{string})"/>
        /// </summary>
        protected Lazy<Maybe<Canvas>> highPriorityCanvas = new(() => new Catched<Canvas>(() => DependencyContainer.Resolve<Canvas>(UnityDependecyID.HighPriorityCanvas)).Maybe());

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
                dragThisCloned = Instantiate(this, cTransform.Value.parent);
                dragThisCloned.IfSome(x =>
                {
                    x.transform.position = cTransform.Value.position;
                    x.Hide(DisableGraphicsSettings.None);
                    //Hides it and disable any interaction with the copy.
                });
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
                    cTransform.Value.SetParent(x.transform);
                });
            }

            //Set sibling index after moving to high priority canvas for correctly overlapping
            if (dragSettings.IsFlagSetted(DragAndDropSettings.SetAsLastSiblingWhenDragging))
                cTransform.Value.SetAsLastSibling();
        }

        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!DragAllowed)
                return;

            cTransform.Value.position = eventData.position;
        }

        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!DragAllowed)
                return;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.InHighPriorityCanvas))
            {
                highPriorityCanvas.Value.IfSome(_ =>
                {
                    startDraggingParent.Match(
                        some: parent => cTransform.Value.SetParent(parent),
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

            dragAllowed = null;
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

        protected ArraySegment<BeforeDisabledGraphicComponentSnapshot> showableDisabledGraphics;
        protected DisableGraphicsSettings showableDisableGraphicsSettings = DisableGraphicsSettings.Default;
        private Subject<Unit>? showableShowSubj;
        private Subject<Unit>? showableHideSubj;

        public bool IsVisible { get; protected set; }
        protected virtual bool showOnStart { get; }

        private void StartIShowable()
        {
            ShowablePreheat();

            if (showOnStart)
                Show();
            else
                Hide(showableDisableGraphicsSettings);
        }

        public virtual void Hide(DisableGraphicsSettings disableGraphicsSettings)
        {
            if (!IsVisible)
                return;

            showableDisabledGraphics = UIHelper.DisableGraphics(cGameObject.Value,
                disableGraphicsSettings == DisableGraphicsSettings.Default
                ?
                showableDisableGraphicsSettings
                : 
                disableGraphicsSettings);

            IsVisible = false;
        }
        public void Hide() => Hide(DisableGraphicsSettings.Default);

        public virtual void Show()
        {
            if (IsVisible)
                return;

            UIHelper.EnableGraphics(showableDisabledGraphics);
            IsVisible = true;
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

        private void ShowablePreheat()
        {
            Show();
            Hide();
        }

        #endregion IShowable

        #region ISelectable

        protected readonly ReactiveProperty<bool> isSelected = new();
        protected Color selectableSelectionColor = Color.red;
        protected Color selectableBeforeSelectColor;
        private Subject<Unit>? selectableSelectSubj;
        private Subject<Unit>? selectableDeselectSubj;

        public IReadOnlyReactiveProperty<bool> IsSelected => isSelected;

        private void StartISelectable()
        {
            SelectablePreheat();
        }

        public virtual bool SelectPredicate(out Maybe<string> msg)
        {
            msg = null;
            return true;
        }

        public virtual void DoSelect()
        {
            if (isSelected.Value)
                return;

            isSelected.Value = true;

            Img.IfSome(img =>
            {
                selectableBeforeSelectColor = img.color;
                img.color *= selectableSelectionColor;
            });
        }

        public virtual void DoDeselect()
        {
            if (!isSelected.Value)
                return;

            isSelected.Value = false;

            Img.IfSome(img =>
            {
                selectableBeforeSelectColor = img.color;
                img.color *= Color.red;
            });
        }

        public void SwitchSelectionState()
        {
            if (isSelected.Value)
                DoDeselect();
            else
                DoSelect();
        }

        public IObservable<Unit> ObserveSelect()
        {
            selectableSelectSubj ??= new Subject<Unit>();

            return selectableSelectSubj;
        }

        public IObservable<Unit> ObserveDeselect()
        {
            selectableDeselectSubj ??= new Subject<Unit>();

            return selectableDeselectSubj;
        }

        private void SelectablePreheat()
        {
            DoSelect();
            DoDeselect();
        }

        #endregion ISelectable
    }
}
