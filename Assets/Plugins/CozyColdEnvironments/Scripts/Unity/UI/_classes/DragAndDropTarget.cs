using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DragAndDropTarget : CCBehaviour, IDragAndDropTarget
    {
        public DragAndDropSettings dragSettings;

        [GetBySelf(IsOptional = true)]
        private IShowable m_Showable = null!;

        private Vector2 startPos;
        private int startSiblingIndex;
        private Maybe<DragAndDropTarget> thisClone;
        private Maybe<Transform> startDraggingParent;

        public Maybe<IShowable> showable => m_Showable.Maybe();

        public bool DragAllowed => DragPredicate();
        protected Lazy<AnonymousDragAndDropTarget> dragAndDrop { get; private set; } = null!;
        /// <summary>
        /// It's cached result of the <see cref="DragAllowedPredicate(out Maybe{string})"/>
        /// </summary>
        protected Lazy<Maybe<Canvas>> highPriorityCanvas = new(() => new Catched(logType: CCEnvs.Diagnostics.LogType.Error).Do(() => BuiltInDependecyContainer.Resolve<Canvas>(UnityDependecyID.HighPriorityCanvas)));

        int IDragAndDropTarget.BindingCount => dragAndDrop.Value.BindingCount;

        protected override void Awake()
        {
            base.Awake();

            dragAndDrop = new Lazy<AnonymousDragAndDropTarget>(
                () => new AnonymousDragAndDropTarget(
                    gameObject,
                    OnBeginDrag,
                    OnDrag,
                    OnEndDrag,
                    OnDrop)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool DragPredicate() => true;

        protected virtual bool DropPredicate() => true;

        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!DragAllowed)
                return;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.ResetPos))
                startPos = cTransform.Value.position;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.RefillEmptySpace))
            {
                thisClone = Instantiate(this, cTransform.Value.parent);
                thisClone.IfSome(x =>
                {
                    x.transform.position = cTransform.Value.position;
                    x.showable.IfSome(cmp => cmp.Hide());
                    //Hides it and disable any interaction with the copy.
                });
            }

            //Save sibling index before it moved to high priority canvas
            //to correctly restore it position
            if (dragSettings.IsFlagSetted(DragAndDropSettings.SetAsLastSiblingWhenDragging))
                startSiblingIndex = transform.GetSiblingIndex();

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
                    startDraggingParent.Do(
                        some: parent => cTransform.Value.SetParent(parent),
                        none: () => this.PrintError("Cannot find previous parent transfrom.")
                        );
                });
            }

            if (dragSettings.IsFlagSetted(DragAndDropSettings.ResetPos))
                cTransform.Value.position = startPos;

            if (dragSettings.IsFlagSetted(DragAndDropSettings.RefillEmptySpace))
                thisClone.IfSome(static x => Destroy(x.gameObject));

            if (dragSettings.IsFlagSetted(DragAndDropSettings.SetAsLastSiblingWhenDragging))
                cTransform.Value.SetSiblingIndex(startSiblingIndex);
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
