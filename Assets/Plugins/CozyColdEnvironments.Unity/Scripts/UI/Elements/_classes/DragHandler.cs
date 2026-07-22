using CCEnvs.Diagnostics;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class DragHandler
        :
        CCBehaviour,
        IDragHandler,
        IElement
    {
        [SerializeField]
        protected string? targetName;

        [SerializeField]
        protected string? dropTargetTag;

        [SerializeField]
        protected LayerMask dropTargetLayerMask = ~0;

        private ReactiveCommand<DragContext>? onBeginDragCmd;
        private ReactiveCommand<DragContext>? onDragCmd;
        private ReactiveCommand<DragContext>? onEndDragCmd;

        public event Action<DragContext>? OnBeginDrag;
        public event Action<DragContext>? OnDrag;
        public event Action<DragContext>? OnEndDrag;

        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }
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
            get => dropTargetLayerMask;
            set => SetDropTargetLayerMask(value);
        }

        bool IToggleable.IsEnabled {
            get => enabled;
            set => enabled = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Renderer.RegisterUIReloadCallback(OnUIReload);    
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IsDragging = false;
            Renderer.UnregisterUIReloadCallback(OnUIReload);
            RendererRoot = null;
            target = null;

            if (target is not null)
            {
                target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }
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

        public DragHandler SetDropTargetLayerMask(int layerMask)
        {
            dropTargetLayerMask = layerMask;
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

        private void OnUIReloadInternal(PanelRenderer _, VisualElement root)
        {
            RendererRoot = root;

            if (targetName.IsNullOrWhiteSpace())
                target = root;
            else
                target = root.Q<VisualElement>(targetName);

            if (target is not null)
            {
                target.RegisterCallback<PointerDownEvent>(OnPointerDown);
                target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }
        }

        private void OnPointerDown(PointerDownEvent ev)
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

            RendererRoot.CapturePointer(context.Event.pointerId);
            OnBeginEvent(ev);
            OnBeginDrag?.Invoke(context);
            onBeginDragCmd?.Execute(context);

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("Begin Drag");
        }

        private void OnPointerMove(PointerMoveEvent ev)
        {
            if (!IsDragging)
                return;

            Guard.IsNotNull(target);

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            OnDragEvent(ev);
            OnDrag?.Invoke(context);
            onDragCmd?.Execute(context);

            if (CCDebug<DragHandler>.IsEnabled && Time.time % 0.5f == 0)
                this.PrintLog("Dragging");
        }

        private void OnPointerUp(PointerUpEvent ev) 
        {
            if (!IsDragging)
                return;

            Guard.IsNotNull(target);

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
                dropTarget.GameObject.Q()
                    .Component<IDropHandler>()
                    .Lax()
                    .TryGetValue(out var targetDropHandler)
                )
            {
                targetDropHandler.SendDropEvent(context);
            }

            RendererRoot.ReleasePointer(context.Event.pointerId);
            OnEndEvent(ev);
            OnEndDrag?.Invoke(context);
            onEndDragCmd?.Execute(context);

            IsDragging = false;

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("End Drag");
        }
    }
}
