using CCEnvs.Diagnostics;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.ComponentInjections;
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
            Renderer.RegisterUIReloadCallback(OnUIReloadInternal);    
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IsDragging = false;
            Renderer.UnregisterUIReloadCallback(OnUIReloadInternal);
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

        protected virtual void OnBeginEvent(DragContext context) { }

        protected virtual void OnDragEvent(DragContext context) { }

        protected virtual void OnEndEvent(DragContext context) { }

        private void OnUIReloadInternal(PanelRenderer renderer, VisualElement root)
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

            try
            {
                OnUIReload(renderer, root);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
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

            try
            {
                OnBeginEvent(context);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnBeginDrag?.Invoke(context);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            onBeginDragCmd?.Execute(context);

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("Begin Drag");
        }

        private void OnPointerMove(PointerMoveEvent ev)
        {
            if (!IsDragging || target is null)
                return;

            var context = DragContext.Create(
                target,
                gameObject,
                ev
                );

            try
            {
                OnDragEvent(context);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnDrag?.Invoke(context);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            onDragCmd?.Execute(context);

            if (CCDebug<DragHandler>.IsEnabled && Time.time % 0.5f == 0)
                this.PrintLog("Dragging");
        }

        private void OnPointerUp(PointerUpEvent ev) 
        {
            if (!IsDragging || target is null)
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
                dropTarget.GameObject.Q()
                    .Component<IDropHandler>()
                    .Lax()
                    .TryGetValue(out var targetDropHandler)
                )
            {
                targetDropHandler.SendDropEvent(context);
            }

            RendererRoot.ReleasePointer(context.Event.pointerId);

            try
            {
                OnEndEvent(context);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnEndDrag?.Invoke(context);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            onEndDragCmd?.Execute(context);

            IsDragging = false;

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("End Drag");
        }
    }
}
