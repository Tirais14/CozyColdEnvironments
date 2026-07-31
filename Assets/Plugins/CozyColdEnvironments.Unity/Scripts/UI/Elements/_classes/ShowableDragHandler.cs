using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Threading;
using CCEnvs.UnityX.Async;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using System.Threading;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.UnityX.UI.Elements
{
    [DisallowMultipleComponent]
    public class ShowableDragHandler : DragHandler
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

        [GetBySelf]
        private IShowableElement showable = null!;

        public VisualElement? GhostRoot { get; private set; }

        public MonoBehaviour? Ghost { get; private set; }

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

        protected void SetGhostRootPosition(Vector2 pos)
        {
            Guard.IsNotNull(GhostRoot);

            switch (dragPosition)
            {
                case DragPosition.LeftTop:
                    GhostRoot.style.left = pos.x;
                    GhostRoot.style.top = pos.y;
                    break;
                case DragPosition.Center:
                    float xOffset = showable.RootElement?.layout.size.x / 2 ?? 0;
                    float yOffset = showable.RootElement?.layout.size.y / 2 ?? 0;
                    GhostRoot.style.left = pos.x - xOffset;
                    GhostRoot.style.top = pos.y - yOffset;
                    break;
                default:
                    throw CC.ThrowHelper.InvalidOperationException(dragPosition);
            }
        }

        protected override void OnBeginDragEvent(DragEvent ev)
        {
            base.OnBeginDragEvent(ev);

            if (!enabled || showable.RootElement is null)
                return;

            Ghost = Instantiate(ghostPrefab.IfNull(gameObject)).AddComponent<EmptyMonoBehaviour>();
            var ghostShowable = Ghost.Q().Component<IShowableElement>().Strict();

            ev.SetTarget(
                ghostShowable.RootElement,
                ghostShowable.As<Component>().IfNotNull(x => x.gameObject)
                );

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

            OnBeginDragCoreAsync(ev).ForgetByPrintException();
        }

        protected override void OnDragEvent(DragEvent ev)
        {
            base.OnDragEvent(ev);

            if (!IsDragging || GhostRoot is null)
                return;

            SetGhostRootPosition(ev.Info.position);
        }

        protected override void OnEndDragEvent(DragEvent ev)
        {
            base.OnEndDragEvent(ev);

            if (IsDragging)
            {
                if (showable.RootElement is not null)
                {
                    if (hideWhenDrag)
                        showable.RootElement.visible = true;

                    if (isMoveToDropPosition && GhostRoot is not null)
                    {
                        StyleEnum<Position> showablePosition = showable.RootElement.style.position;
                        showable.RootElement.style.position = Position.Absolute;
                        showable.RootElement.style.left = GhostRoot.style.left;
                        showable.RootElement.style.right = GhostRoot.style.right;
                        showable.RootElement.style.position = showablePosition;
                    }
                }
            }

            GhostRoot = null;

            if (Ghost != null)
                Destroy(Ghost.gameObject);

            Ghost = null;
        }

        private async UniTask OnBeginDragCoreAsync(DragEvent ev)
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
                    static showableClone => showableClone.RootElement is not null,
                    timing: PlayerLoopTiming.PreUpdate,
                    cancellationToken: linkedCancellationToken
                    );

                if (CCDebug<ShowableDragHandler>.IsEnabled)
                    task = task.Timeout(60.Seconds());

                await task;
            }

            GhostRoot = showableClone.RootElement.ThrowIfNull(nameof(showableClone.RootElement));
            GhostRoot.pickingMode = PickingMode.Ignore;
            GhostRoot.style.position = Position.Absolute;
            SetGhostRootPosition(ev.Info.position);

            if (showable.RootElement is not null && hideWhenDrag)
                showable.RootElement.visible = false;
        }
    }
}
