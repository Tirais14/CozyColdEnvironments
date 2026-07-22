using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using UnityEngine.UIElements;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.UnityX.UI.Elements
{
    public class ShowableDragHandler : CCBehaviour
    {
        [GetByParent]
        private IDragHandler handler = null!;

        [GetBySelf]
        private IShowableElement showable = null!;

        private VisualElement? showableClone;

        private bool isDragging;

        protected override void OnEnable()
        {
            base.OnEnable();
            handler.OnBeginDrag += OnBeginDrag;
            handler.OnDrag += OnDrag;
            handler.OnEndDrag += OnEndDrag;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            handler.OnBeginDrag -= OnBeginDrag;
            handler.OnDrag -= OnDrag;
            handler.OnEndDrag -= OnEndDrag;
        }

        private void OnBeginDrag(DragContext context)
        {
            if (!enabled || showable.RendererRoot is null)
                return;

            showableClone = showable.Renderer.visualTreeAsset.CloneTree();
            showableClone.style.position = Position.Absolute;
            //showable.RendererRoot.visible = false;
            showable.RendererRoot.CapturePointer(context.Event.pointerId);
            isDragging = true;
        }

        private void OnDrag(DragContext context)
        {
            if (!isDragging || showableClone is null)
                return;

            showableClone.style.left = context.Event.position.x;
            showableClone.style.top = context.Event.position.y;
        }

        private void OnEndDrag(DragContext context)
        {
            if (!isDragging || showableClone is null)
                return;

            if (showable.RendererRoot is not null)
            {
                showable.RendererRoot.visible = true;
                showable.RendererRoot.ReleasePointer(context.Event.pointerId);
            }

            showableClone.RemoveFromHierarchy();
            showableClone = null;
            isDragging = false;
        }
    }
}
