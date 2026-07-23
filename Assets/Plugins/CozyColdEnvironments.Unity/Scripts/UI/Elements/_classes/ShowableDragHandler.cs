using CCEnvs.Diagnostics;
using CCEnvs.Threading;
using CCEnvs.UnityX.Async;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using Cysharp.Threading.Tasks;
using Humanizer;
using System.Threading;
using UnityEngine;
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

        private VisualElement? showableCloneElement;

        private ShowableDragHandler? clone;

        public bool IsDragging { get; private set; }

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

        private async UniTask OnBeginDragCoreAsync()
        {
            if (clone == null)
                return;

            var showableClone = clone.Q().Component<IShowableElement>().Strict();

            using (var linkedCancellationTokenSource = destroyCancellationToken.TryLinkTokens(
                clone.destroyCancellationToken,
                out CancellationToken linkedCancellationToken
                ))
            {
                await UniTask.WaitUntil(
                    showableClone,
                    static showableClone => showableClone.RendererRoot is not null,
                    timing: PlayerLoopTiming.PreUpdate,
                    cancellationToken: linkedCancellationToken
                    )
                    .Timeout(60.Seconds());
            }

            showableCloneElement = showableClone.RendererRoot.ThrowIfNull(nameof(showableClone.RendererRoot));
            showableCloneElement.pickingMode = PickingMode.Ignore;
            showableCloneElement.style.position = Position.Absolute;
            IsDragging = true;
        }

        private void OnBeginDrag(DragContext context)
        {
            if (!enabled || showable.RendererRoot is null)
                return;

            clone = Instantiate(this, showable.As<Component>().IfNotNull(x => x.transform));

            Destroy(clone.Q().Component<ShowableDragHandler>().Strict());

            clone.Q()
                .Component<IDropHandler>()
                .Lax()
                .Cast<Component>()
                .Do(dropHandler => Destroy(dropHandler));

            clone.Q()
                .Component<IDragHandler>()
                .Lax()
                .Cast<Component>()
                .Do(dragHandler => Destroy(dragHandler));   

            OnBeginDragCoreAsync().ForgetByPrintException();
        }

        private void OnDrag(DragContext context)
        {
            if (!IsDragging || showableCloneElement is null)
                return;

            showableCloneElement.style.left = context.Event.position.x;
            showableCloneElement.style.top = context.Event.position.y;
        }

        private void OnEndDrag(DragContext context)
        {
            if (!IsDragging || showableCloneElement is null)
                return;

            if (showable.RendererRoot is not null)
                showable.RendererRoot.visible = true;

            showableCloneElement.RemoveFromHierarchy();
            showableCloneElement = null;

            if (clone != null)
                Destroy(clone);

            IsDragging = false;
        }
    }
}
