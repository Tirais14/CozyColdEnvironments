using CCEnvs.Attributes;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
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

        private bool isDragging;

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

        public override bool IsDragging => isDragging && base.IsDragging;

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

            Ghost = Instantiate(
                ghostPrefab.IfNull(gameObject),
                showable.Root.As<Component>().IfNotNull(root => root.transform)
                )
                .AddComponent<EmptyMonoBehaviour>();

            var ghostShowable = Ghost.Q().Component<IShowableElement>().Strict();

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

            ghostShowable.ObserveRootElement()
                .Where(root => root is not null)
                .Take(1)
                .Timeout(60.Seconds())
                .Subscribe(
                (this, ev, ghostShowable),
                static (root, args) =>
                {
                    var (@this, ev, ghostShowable) = args;

                    ev.SetTarget(
                        ghostShowable.RootElement,
                        ghostShowable.As<Component>().IfNotNull(x => x.gameObject)
                        );

                    @this.GhostRoot = root!;
                    @this.GhostRoot.pickingMode = PickingMode.Ignore;
                    @this.GhostRoot.style.position = Position.Absolute;
                    @this.SetGhostRootPosition(ev.Info.position);

                    if (@this.showable.RootElement is not null && @this.hideWhenDrag)
                        @this.showable.RootElement.style.display = DisplayStyle.None;

                    @this.isDragging = true;
                });
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
                        showable.RootElement.style.display = DisplayStyle.Flex;

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
    }
}
