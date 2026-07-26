using CCEnvs.Attributes;
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
    [DisallowMultipleComponent]
    public class ShowableDragHandler : CCBehaviour
    {
        [SerializeField, OptionalField]
        [Tooltip("Must contains " + nameof(IShowableElement))]
        protected GameObject? ghostPrefab;

        [SerializeField]
        protected DragPosition dragPosition = DragPosition.Center;

        [SerializeField]
        protected bool isMoveToDropPosition;
        [SerializeField]
        protected bool hideWhenDrag;

        [GetByParent]
        private IDragHandler handler = null!;

        [GetBySelf]
        private IShowableElement showable = null!;

        public VisualElement? ShowableGhostElement { get; private set; }

        public MonoBehaviour? Ghost { get; private set; }

        public bool IsDragging { get; private set; }
        public bool HideWhenDrag {
            get => hideWhenDrag;
            set => SetHideWhenDrag(value);
        }
        public bool IsMoveToDropPosition {
            get => isMoveToDropPosition;
            set => SetIsMoveToDropPosition(value);
        }

        public DragPosition DragPosition {
            get => dragPosition;
            set => SetDragPosition(value);
        }

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

        public ShowableDragHandler SetDragPosition(DragPosition value)
        {
            if (value == DragPosition.None)
                throw new System.ArgumentException(nameof(value));   

            dragPosition = value;
            return this;
        }

        public ShowableDragHandler SetIsMoveToDropPosition(bool state)
        {
            isMoveToDropPosition = state;
            return this;
        }

        public ShowableDragHandler SetHideWhenDrag(bool state)
        {
            hideWhenDrag = state;
            return this;
        }

        private async UniTask OnBeginDragCoreAsync()
        {
            if (Ghost == null)
                return;

            var showableClone = Ghost.Q().Component<IShowableElement>().Strict();

            using (var linkedCancellationTokenSource = destroyCancellationToken.TryLinkTokens(
                Ghost.destroyCancellationToken,
                out CancellationToken linkedCancellationToken
                ))
            {
                var task = UniTask.WaitUntil(
                    showableClone,
                    static showableClone => showableClone.RendererRoot is not null,
                    timing: PlayerLoopTiming.PreUpdate,
                    cancellationToken: linkedCancellationToken
                    );

                if (CCDebug<ShowableDragHandler>.IsEnabled)
                    task = task.Timeout(60.Seconds());

                await task;
            }

            ShowableGhostElement = showableClone.RendererRoot.ThrowIfNull(nameof(showableClone.RendererRoot));
            ShowableGhostElement.pickingMode = PickingMode.Ignore;
            ShowableGhostElement.style.position = Position.Absolute;

            if (showable.RendererRoot is not null && hideWhenDrag)
                showable.RendererRoot.visible = false;

            IsDragging = true;
        }

        private void OnBeginDrag(DragEvent context)
        {
            if (!enabled || showable.RendererRoot is null)
                return;

            Ghost = Instantiate(ghostPrefab.IfNull(gameObject)).AddComponent<EmptyMonoBehaviour>();

            Destroy(Ghost.Q().Component<ShowableDragHandler>().Strict());

            Ghost.Q()
                .Component<IDropHandler>()
                .Lax()
                .Cast<Component>()
                .Do(dropHandler => Destroy(dropHandler));

            Ghost.Q()
                .Component<IDragHandler>()
                .Lax()
                .Cast<Component>()
                .Do(dragHandler => Destroy(dragHandler));   

            OnBeginDragCoreAsync().ForgetByPrintException();
        }

        private void OnDrag(DragEvent context)
        {
            if (!IsDragging || ShowableGhostElement is null)
                return;

            switch (dragPosition)
            {
                case DragPosition.LeftTop:
                    ShowableGhostElement.style.left = context.Event.position.x;
                    ShowableGhostElement.style.top = context.Event.position.y;
                    break;
                case DragPosition.Center:
                    float xOffset = showable.RendererRoot?.layout.size.x / 2 ?? 0;
                    float yOffset = showable.RendererRoot?.layout.size.y / 2 ?? 0;
                    ShowableGhostElement.style.left = context.Event.position.x - xOffset;
                    ShowableGhostElement.style.top = context.Event.position.y - yOffset;
                    break;
                default:
                    throw CC.ThrowHelper.InvalidOperationException(dragPosition);
            }
        }

        private void OnEndDrag(DragEvent context)
        {
            if (IsDragging)
            {
                if (showable.RendererRoot is not null)
                {
                    if (hideWhenDrag)
                        showable.RendererRoot.visible = true;

                    if (isMoveToDropPosition && ShowableGhostElement is not null)
                    {
                        StyleEnum<Position> showablePosition = showable.RendererRoot.style.position;
                        showable.RendererRoot.style.position = Position.Absolute;
                        showable.RendererRoot.style.left = ShowableGhostElement.style.left;
                        showable.RendererRoot.style.right = ShowableGhostElement.style.right;
                        showable.RendererRoot.style.position = showablePosition;
                    }
                }

                IsDragging = false;
            }

            ShowableGhostElement = null;

            if (Ghost != null)
                Destroy(Ghost.gameObject);

            Ghost = null;
        }
    }
}
