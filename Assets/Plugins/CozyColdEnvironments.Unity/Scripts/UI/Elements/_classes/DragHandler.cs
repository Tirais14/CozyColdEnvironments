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
    public class DragHandler<TBeginEvent, TDragEvent, TEndEvent>
        :
        CCBehaviour,
        IDragHandler<TBeginEvent, TDragEvent, TEndEvent>

        where TBeginEvent : EventBase<TBeginEvent>, new()
        where TDragEvent : EventBase<TDragEvent>, new()
        where TEndEvent : EventBase<TEndEvent>, new()
    {
        [SerializeField]
        protected string? targetName;

        [SerializeField]
        protected string? dropTargetTag;

        [SerializeField]
        protected SerializedNullable<LayerMask> dropTargetLayerMask;

        private ReactiveCommand<DragContext<TBeginEvent>>? onBeginDragCmd;
        private ReactiveCommand<DragContext<TDragEvent>>? onDragCmd;
        private ReactiveCommand<DragContext<TEndEvent>>? onEndDragCmd;

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

        protected override void OnEnable()
        {
            base.OnEnable();
            BindUIReload();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
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

        public DragHandler<TBeginEvent, TDragEvent, TEndEvent> SetTargetName(string? name)
        {
            targetName = name;
            return this;
        }

        public DragHandler<TBeginEvent, TDragEvent, TEndEvent> SetDropTargetTag(string? tag)
        {
            dropTargetTag = tag;
            return this;
        }

        public DragHandler<TBeginEvent, TDragEvent, TEndEvent> SetDropTargetLayerMask(int? layerMask)
        {
            dropTargetLayerMask = new SerializedNullable<LayerMask>(layerMask ?? null);
            return this;
        }

        public Observable<DragContext<TBeginEvent>> ObserveBeginDrag()
        {
            onBeginDragCmd ??= new ReactiveCommand<DragContext<TBeginEvent>>();
            return onBeginDragCmd;
        }

        public Observable<DragContext<TDragEvent>> ObserveDrag()
        {
            onDragCmd ??= new ReactiveCommand<DragContext<TDragEvent>>();
            return onDragCmd;
        }

        public Observable<DragContext<TEndEvent>> ObserveEndDrag()
        {
            onEndDragCmd ??= new ReactiveCommand<DragContext<TEndEvent>>();
            return onEndDragCmd;
        }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        protected virtual void OnBeginEvent(TBeginEvent ev) { }

        protected virtual void OnDragEvent(TDragEvent ev) { }

        protected virtual void OnEndEvent(TEndEvent ev) { }

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

        private void OnBeginEventInternal(TBeginEvent ev)
        {
            Guard.IsNotNull(target);
            IsDragging = true;

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            OnBeginEvent(ev);
            onBeginDragCmd?.Execute(context);
        }

        private void BindBeginEvent(VisualElement target)
        {
            target.RegisterCallback<TBeginEvent>(OnBeginEventInternal);
        }

        private void UnbindBeginEvent()
        {
            target?.UnregisterCallback<TBeginEvent>(OnBeginEventInternal);
        }

        private void OnDragEventInternal(TDragEvent ev)
        {
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
            target.RegisterCallback<TDragEvent>(OnDragEventInternal);
        }

        private void UnbindMoveEvent()
        {
            target?.UnregisterCallback<TDragEvent>(OnDragEventInternal);
        }

        private void OnEndEventInternal(TEndEvent ev) 
        {
            Guard.IsNotNull(target);

            if (!IsDragging)
                return;

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            OnEndEvent(ev);
            onEndDragCmd?.Execute(context);

            IsDragging = false;

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
        }

        private void BindEndEvent(VisualElement target)
        {
            target.RegisterCallback<TEndEvent>(OnEndEventInternal);
        }

        private void UnbindEndEvent()
        {
            target?.UnregisterCallback<TEndEvent>(OnEndEventInternal);
        }
    }
}
