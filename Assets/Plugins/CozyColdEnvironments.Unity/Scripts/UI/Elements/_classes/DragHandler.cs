using CCEnvs.Diagnostics;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.EditorSerialization;
using CCEnvs.UnityX.Injections;
using CommunityToolkit.Diagnostics;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class DragHandler
        :
        CCBehaviour,
        IDragHandler
    {
        [SerializeField]
        protected string? targetName;

        [SerializeField]
        protected string? dropTargetTag;

        [SerializeField]
        protected SerializedNullable<LayerMask> dropTargetLayerMask;

        private ReactiveCommand<DragContext>? onBeginDragCmd;
        private ReactiveCommand<DragContext>? onDragCmd;
        private ReactiveCommand<DragContext>? onEndDragCmd;

        [field: GetBySelf]
        public PanelRenderer renderer { get; private set; } = null!;

        public VisualElement? root { get; private set; }
        public VisualElement? target { get; private set; }

        public bool IsDragging { get; private set; }

        public string? TargetName {
            get => targetName;
            set => SetTargetName(value);
        }
        public string? DropTargetTag {
            get => dropTargetTag;
            set => SetDropTargetTag(value);
        }

        public int DropTargetLayerMask {
            get => dropTargetLayerMask.Data ?? ~0;
            set => SetDropTargetLayerMask(value);
        }

        bool IToggleable.IsEnabled {
            get => enabled;
            set => enabled = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BindUIReload();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IsDragging = false;
            UnbindUIReload();
            UnbindBeginEvent();
            UnbindMoveEvent();
            UnbindEndEvent();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onDragCmd?.Dispose();
        }

        public DragHandler SetTargetName(string? name)
        {
            targetName = name;
            return this;
        }

        public DragHandler SetDropTargetTag(string? tag)
        {
            dropTargetTag = tag;
            return this;
        }

        public DragHandler SetDropTargetLayerMask(int? layerMask)
        {
            dropTargetLayerMask = new SerializedNullable<LayerMask>(layerMask ?? null);
            return this;
        }

        public Observable<DragContext> ObserveBeginDrag()
        {
            onBeginDragCmd ??= new ReactiveCommand<DragContext>();
            return onBeginDragCmd;
        }

        public Observable<DragContext> ObserveDrag()
        {
            onDragCmd ??= new ReactiveCommand<DragContext>();
            return onDragCmd;
        }

        public Observable<DragContext> ObserveEndDrag()
        {
            onEndDragCmd ??= new ReactiveCommand<DragContext>();
            return onEndDragCmd;
        }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        protected virtual void OnBeginEvent(PointerDownEvent ev) { }

        protected virtual void OnDragEvent(PointerMoveEvent ev) { }

        protected virtual void OnEndEvent(PointerUpEvent ev) { }

        private void OnUIReloadInternal(PanelRenderer renderer, VisualElement root)
        {
            this.root = root;

            if (targetName.IsNullOrWhiteSpace())
                target = root;
            else
                target = root.Q<VisualElement>(targetName);

            if (target is not null)
            {
                BindBeginEvent(target);
                BindMoveEvent(target);
                BindEndEvent(target);
            }
        }

        private void BindUIReload()
        {
            renderer.RegisterUIReloadCallback(OnUIReloadInternal);
        }

        private void UnbindUIReload()
        {
            renderer.UnregisterUIReloadCallback(OnUIReloadInternal);
            root = null;
            target = null;
        }

        private void OnBeginEventInternal(PointerDownEvent ev)
        {
            if (!enabled)
                return;

            Guard.IsNotNull(target);
            IsDragging = true;

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            OnBeginEvent(ev);
            onBeginDragCmd?.Execute(context);

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("Begin Drag");
        }

        private void BindBeginEvent(VisualElement target)
        {
            target.RegisterCallback<PointerDownEvent>(OnBeginEventInternal);
        }

        private void UnbindBeginEvent()
        {
            target?.UnregisterCallback<PointerDownEvent>(OnBeginEventInternal);
        }

        private void OnDragEventInternal(PointerMoveEvent ev)
        {
            if (!enabled)
                return;

            Guard.IsNotNull(target);

            if (!IsDragging)
                return;

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            OnDragEvent(ev);
            onDragCmd?.Execute(context);
        }

        private void BindMoveEvent(VisualElement target)
        {
            target.RegisterCallback<PointerMoveEvent>(OnDragEventInternal);
        }

        private void UnbindMoveEvent()
        {
            target?.UnregisterCallback<PointerMoveEvent>(OnDragEventInternal);
        }

        private void OnEndEventInternal(PointerUpEvent ev) 
        {
            if (!enabled)
                return;

            Guard.IsNotNull(target);

            if (!IsDragging)
                return;

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            if (DropTargetRegistry.Targets.TryGetValue(ev.target, out DropTarget dropTarget)
                &&
                gameObject != dropTarget.GameObject
                &&
                (dropTargetTag.IsNullOrWhiteSpace() || dropTarget.GameObject.CompareTag(dropTargetTag))
                &&
                (DropTargetLayerMask & (1 << dropTarget.GameObject.layer)) != 0
                &&
                dropTarget.GameObject.Q().Component<IDropHandler>().Lax().TryGetValue(out var targetDropHandler))
            {
                targetDropHandler.SendDropEvent(context);
            }

            OnEndEvent(ev);
            onEndDragCmd?.Execute(context);

            IsDragging = false;

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("End Drag");
        }

        private void BindEndEvent(VisualElement target)
        {
            target.RegisterCallback<PointerUpEvent>(OnEndEventInternal);
        }

        private void UnbindEndEvent()
        {
            target?.UnregisterCallback<PointerUpEvent>(OnEndEventInternal);
        }
    }
}
