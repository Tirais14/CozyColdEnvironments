using CCEnvs.Diagnostics;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public delegate void DragAction(DragEvent ev);

    [DisallowMultipleComponent]
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

        private IDragPredicate? predicate;

        public event DragAction? OnBeginDrag;
        public event DragAction? OnDrag;
        public event DragAction? OnEndDrag;

        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }
        public VisualElement? Target { get; private set; }

        public bool IsDragging { get; private set; }

        public IDragPredicate? Predicate {
            get => predicate;
            set => SetPredicate(value);
        }

        protected DragEvent Event { get; private set; } = null!;

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
            Target = null;

            if (Target is not null)
            {
                Target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                Target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                Target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }
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

        public DragHandler SetPredicate(IDragPredicate? value)
        {
            predicate = value;
            return this;
        }

        public void Refresh()
        {
            enabled = !enabled;
            enabled = !enabled;
        }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        protected virtual void OnBeginEvent(DragEvent context) { }

        protected virtual void OnDragEvent(DragEvent context) { }

        protected virtual void OnEndEvent(DragEvent context) { }

        private void OnUIReloadInternal(PanelRenderer renderer, VisualElement root)
        {
            RendererRoot = root;

            if (targetName.IsNullOrWhiteSpace())
                Target = root;
            else
                Target = root.Q<VisualElement>(targetName);

            if (Target is not null)
            {
                Target.RegisterCallback<PointerDownEvent>(OnPointerDown);
                Target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                Target.RegisterCallback<PointerUpEvent>(OnPointerUp);
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
            if (!enabled || Target is null || (!Predicate?.Evaluate() ?? false))
                return;

            Guard.IsNotNull(Target);
            IsDragging = true;

            Event.SetSource(Target, gameObject)
                .SetTarget(Target, gameObject);

            RendererRoot.CapturePointer(ev.pointerId);

            try
            {
                OnBeginEvent(Event);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnBeginDrag?.Invoke(Event);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("Begin Drag");
        }

        private void OnPointerMove(PointerMoveEvent ev)
        {
            if (!IsDragging || Target is null)
                return;

            Event.SetSource(Target, gameObject);

            try
            {
                OnDragEvent(Event);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnDrag?.Invoke(Event);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void OnPointerUp(PointerUpEvent ev) 
        {
            if (!IsDragging || Target is null)
                return;

            Event.SetSource(Target, gameObject);

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
                targetDropHandler.SendDropEvent(Event);
            }

            RendererRoot.ReleasePointer(ev.pointerId);

            try
            {
                OnEndEvent(Event);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnEndDrag?.Invoke(Event);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            IsDragging = false;

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("End Drag");
        }
    }
}
