using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using CCEnvs.UnityX.UI;
using CCEnvs.UnityX.UI.Elements;
using R3;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    public sealed class ItemContainerDragAndDropHandler : CCBehaviour
    {
        [SerializeField]
        [Tooltip("Must contains View with " + nameof(IItemContainerViewModel) + " and " + nameof(IItemContainer) + " as model")]
        private GameObject containerProxy = null!;

        [GetBySelf]
        private IView containerView = null!;

        [GetBySelf]
        private UnityX.UI.Elements.IDragHandler dragHandler = null!;

        private IDisposable? dragBinging;

        protected override void Awake()
        {
            base.Awake();
            ThrowIfInvalidModel();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BindDragHandler();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dragBinging?.Dispose();
        }

        private void ThrowIfInvalidModel()
        {
            if (!containerView.HasModel<IItemContainer>())
                throw new ArgumentException($"View not contains model or view model of {nameof(ItemContainer)}");
        }

        private void OnBeginDrag(DragContext context)
        {
            if (context.Event)

            Vector2 screenPos = context.Event switch
            {
                MouseDownEvent
                _ => throw CC.ThrowHelper.InvalidOperationException(context.Event?.GetType().FullName ?? "null")
            };
        }

        private void BindDragHandler()
        {
            dragBinging = dragHandler.ObserveDrag().Subscribe(OnBeginDrag);
        }
    }
}
