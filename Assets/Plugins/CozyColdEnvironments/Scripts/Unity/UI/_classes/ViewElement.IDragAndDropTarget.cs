using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Dependencies;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public partial class ViewElement : IDragAndDropTarget
    {
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
            Default = None,
        }

        [Space]
        [Header(nameof(DragAndDropTarget) + " Settings")]
        [Space]

        [SerializeField]
        protected DragAndDropSettings dragSettings = DragAndDropSettings.Default;
        private Vector2 dragStartPos;
        private int dragStartSiblingIndex;
        private Maybe<ViewElement> dragThisCloned;
        private Maybe<Transform> startDraggingParent;
        private bool? dragAllowed;

        public bool DragAllowed {
            get => dragAllowed ?? DragAllowedPredicate();
        }
        protected Lazy<DragAndDropTarget> dragAndDrop { get; private set; } = null!;
        /// <summary>
        /// It's cached result of the <see cref="DragAllowedPredicate(out Maybe{string})"/>
        /// </summary>
        protected Lazy<Maybe<Canvas>> highPriorityCanvas = new(() => new Catched(logType: CCEnvs.Diagnostics.LogType.Error).Do(() => DependencyContainer.Resolve<Canvas>(UnityDependecyID.HighPriorityCanvas)));

        int IDragAndDropTarget.BindingCount => dragAndDrop.Value.BindingCount;

        private void AwakeIDragAndDropTarget()
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

        public virtual bool DragAllowedPredicate() => true;

        public virtual bool DropAllowedPredicate() => false;

        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!DragAllowedPredicate())
            {
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
                    x.Hide(IShowable.Settings.None);
                    //Hides it and disable any interaction with the copy.
                });
            }

            //Save sibling index before it moved to high priority canvas
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

        void IDragAndDropTarget.ActivateDragAndDropAbility()
        {
            dragAndDrop.Value.ActivateDragAndDropAbility();
        }

        void IDragAndDropTarget.DeactivateDragAndDropAbility()
        {
            dragAndDrop.Value.DeactivateDragAndDropAbility();
        }
    }
}
