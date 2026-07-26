using CCEnvs.Diagnostics;
using CCEnvs.Pools;
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
        protected string? dropTargetTag;

        [SerializeField]
        protected LayerMask dropTargetLayerMask = ~0;

        private readonly DragEvent dragEv = new();

        private IDragPredicate? predicate;

        public event DragAction? OnBeginDrag;
        public event DragAction? OnDrag;
        public event DragAction? OnEndDrag;

        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }

        public bool IsDragging { get; private set; }

        public IDragPredicate? Predicate {
            get => predicate;
            set => SetPredicate(value);
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

            if (RendererRoot is not null)
            {
                RendererRoot.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                RendererRoot.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                RendererRoot.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }

            RendererRoot = null;
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

        protected virtual void OnBeginDragEvent(DragEvent ev) { }

        protected virtual void OnDragEvent(DragEvent ev) { }

        protected virtual void OnEndDragEvent(DragEvent ev) { }

        private void OnUIReloadInternal(PanelRenderer renderer, VisualElement root)
        {
            RendererRoot = root;

            root.RegisterCallback<PointerDownEvent>(OnPointerDown);
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            root.RegisterCallback<PointerUpEvent>(OnPointerUp);

            try
            {
                OnUIReload(renderer, root);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void OnPointerDown(PointerDownEvent pointerEv)
        {
            if (!enabled ||
                RendererRoot is null ||
                (!Predicate?.Evaluate() ?? false))
            {
                return;
            }

            IsDragging = true;
            RendererRoot.CapturePointer(pointerEv.pointerId);
            dragEv.SetSource(RendererRoot, gameObject)
                .SetTarget(RendererRoot, gameObject)
                .SetInfo(pointerEv);

            try
            {
                OnBeginDragEvent(dragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnBeginDrag?.Invoke(dragEv);
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
            if (!IsDragging ||
                RendererRoot is null)
            {
                return;
            }

            dragEv.SetSource(RendererRoot, gameObject)
                .SetInfo(ev);

            try
            {
                OnDragEvent(dragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnDrag?.Invoke(dragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void OnPointerUp(PointerUpEvent ev) 
        {
            if (!IsDragging || 
                RendererRoot is null ||
                dragEv is null)
            {
                return;
            }

            dragEv.SetSource(RendererRoot, gameObject)
                .SetInfo(ev);

            if (DropTargetRegistry.Targets.TryGetValue(ev.target, out DropTarget dropTarget) &&
                gameObject != dropTarget.GameObject &&
                (dropTargetTag.IsNullOrWhiteSpace() || dropTarget.GameObject.CompareTag(dropTargetTag)) &&
                (DropTargetLayerMask & (1 << dropTarget.GameObject.layer)) != 0 &&
                dropTarget.GameObject.Q()
                    .Component<IDropHandler>()
                    .Lax()
                    .TryGetValue(out var targetDropHandler)
                )
            {
                targetDropHandler.SendDropEvent(dragEv);
            }

            RendererRoot.ReleasePointer(ev.pointerId);

            try
            {
                OnEndDragEvent(dragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnEndDrag?.Invoke(dragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            IsDragging = false;

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("End Drag");

            dragEv.SetTarget(null, null);
        }
    }
}
