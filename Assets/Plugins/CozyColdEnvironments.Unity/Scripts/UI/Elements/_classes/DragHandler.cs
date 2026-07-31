using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public delegate void DragAction(DragEvent ev);

    [DisallowMultipleComponent]
    public class DragHandler
        :
        CCBehaviour,
        IDragHandler
    {
        [SerializeField]
        protected string? dropTargetTag;

        [SerializeField]
        protected LayerMask dropTargetLayerMask = ~0;

        private readonly DragEvent dragEv = new();

        private IDragPredicate? predicate;

        private IDisposable? rootElementBinding;

        public event DragAction? OnBeginDrag;
        public event DragAction? OnDrag;
        public event DragAction? OnEndDrag;

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

        [field: GetBySelf]
        protected IElement element { get; private set; } = null!;

        bool IToggleable.IsEnabled {
            get => enabled;
            set => enabled = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            element.ObserveRootElement().Subscribe(OnRootElementChangedInternal);   
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IsDragging = false;
            CCDisposable.Dispose(ref rootElementBinding);
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

        protected virtual void OnBeginDragEvent(DragEvent ev) { }

        protected virtual void OnDragEvent(DragEvent ev) { }

        protected virtual void OnEndDragEvent(DragEvent ev) { }

        protected virtual void OnRootElementChanged(RootElementChangedEvent root) { }

        private void OnRootElementChangedInternal(RootElementChangedEvent root)
        {
            if (root.Previous is not null)
            {
                root.Previous.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                root.Previous.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                root.Previous.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }

            if (root.Current is not null)
            {
                root.Current.RegisterCallback<PointerDownEvent>(OnPointerDown);
                root.Current.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                root.Current.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }

            try
            {
                OnRootElementChanged(root);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void OnPointerDown(PointerDownEvent pointerEv)
        {
            if (!enabled ||
                element.RootElement is null ||
                (!Predicate?.Evaluate() ?? false))
            {
                return;
            }

            IsDragging = true;
            element.RootElement.CapturePointer(pointerEv.pointerId);
            dragEv.SetSource(element.RootElement, gameObject)
                .SetTarget(element.RootElement, gameObject)
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
                element.RootElement is null)
            {
                return;
            }

            dragEv.SetSource(element.RootElement, gameObject)
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
                element.RootElement is null ||
                dragEv is null)
            {
                return;
            }

            dragEv.SetSource(element.RootElement, gameObject)
                .SetInfo(ev);

            if (DropTargetRegistry.Targets.TryGetValue(ev.currentTarget, out DropTarget dropTarget) &&
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

            element.RootElement.ReleasePointer(ev.pointerId);

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
