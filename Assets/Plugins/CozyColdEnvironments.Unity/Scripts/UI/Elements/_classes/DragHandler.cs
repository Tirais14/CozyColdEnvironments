using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Pools;
using CCEnvs.TypeMatching;
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

        [SerializeField]
        protected int pointerButton = 0;

        [SerializeField]
        protected float dragThreshold = 0.2f;

        private readonly DragEvent dragEv = new();

        [GetBySelf]
        private IShowableElement showable = null!;

        private IDragPredicate? predicate;

        private float secondsSincePointerDown;

        private bool isPointerLeaved;

        private IDisposable? rootElementBinding;
        private LightDisposable<(DragHandler, VisualElement)> pointerDownRegistration;
        private LightDisposable<(DragHandler, VisualElement)> pointerMoveRegistration;
        private LightDisposable<(DragHandler, VisualElement)> pointerUpRegistration;
        private LightDisposable<(DragHandler, VisualElement)> pointerLeaveRegistration;

        public event DragAction? OnBeginDrag;
        public event DragAction? OnDrag;
        public event DragAction? OnEndDrag;

        public virtual bool IsDragging { get; private set; }

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
        public int PointerButton {
            get => pointerButton;
            set => SetPointerButton(value);
        }

        [field: GetBySelf]
        protected IElement element { get; private set; } = null!;

        bool IToggleable.IsEnabled {
            get => enabled;
            set => enabled = value;
        }

        protected override void Start()
        {
            base.Start();
            rootElementBinding = element.ObserveRootElement().Subscribe(OnRootElementChangedInternal);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            IsDragging = false;
            CCDisposable.Dispose(ref rootElementBinding);
            ClearRootElementBindings();
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

        public DragHandler SetPointerButton(int value)
        {
            pointerButton = value;
            return this;
        }

        protected virtual void OnBeginDragEvent(DragEvent ev) { }

        protected virtual void OnDragEvent(DragEvent ev) { }

        protected virtual void OnEndDragEvent(DragEvent ev) { }

        protected virtual void OnRootElementChanged(VisualElement? root) { }

        private void ClearRootElementBindings()
        {
            pointerDownRegistration.Dispose();
            pointerMoveRegistration.Dispose();
            pointerUpRegistration.Dispose();
            pointerLeaveRegistration.Dispose();

            pointerDownRegistration = default;
            pointerMoveRegistration = default;
            pointerUpRegistration = default;
            pointerLeaveRegistration = default;
        }

        private void OnPointerDown(PointerDownEvent pointerEv)
        {
            if (!enabled ||
                pointerEv.button != pointerButton ||
                element.RootElement is null ||
                (!Predicate?.Evaluate() ?? false))
            {
                return;
            }

            element.RootElement.CapturePointer(pointerEv.pointerId);
            dragEv.SetSource(element.RootElement, gameObject)
                .SetTarget(element.RootElement, gameObject)
                .SetInfo(pointerEv);

            IsDragging = secondsSincePointerDown > dragThreshold;
            isPointerLeaved = false;

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
            if (isPointerLeaved || element.RootElement is null)
                return;

            secondsSincePointerDown += Time.unscaledDeltaTime;
            IsDragging = secondsSincePointerDown > dragThreshold;

            if (!IsDragging)
                return;

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

            ev.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent ev)
        {
            secondsSincePointerDown = 0f;

            if (isPointerLeaved ||
                !IsDragging ||
                element.RootElement is null ||
                dragEv is null)
            {
                return;
            }

            dragEv.SetSource(element.RootElement, gameObject)
                .SetInfo(ev);

            using (var dropElements = new PooledList<VisualElement>(null))
            {
                if (showable.Root.IfNull(showable).RootElement.Is(out VisualElement? root))
                {
                    root.panel.PickAll(ev.position, dropElements);

                    for (int i = 0; i < dropElements.Count; i++)
                    {
                        VisualElement dropElement = dropElements[i];

                        if (DropTargetRegistry.Targets.TryGetValue(dropElement, out DropTarget dropTarget) &&
                            gameObject != dropTarget.GameObject &&
                            (dropTargetTag.IsNullOrWhiteSpace() || dropTarget.GameObject.CompareTag(dropTargetTag)) &&
                            (DropTargetLayerMask & (1 << dropTarget.GameObject.layer)) != 0 &&
                            dropTarget.GameObject.Q()
                                .Component<IDropHandler>()
                                .Lax()
                                .TryGetValue(out var targetDropHandler)
                            )
                        {
                            try
                            {
                                targetDropHandler.SendDropEvent(dragEv);
                            }
                            catch (Exception ex)
                            {
                                this.PrintException(ex);
                            }

                            break;
                        }
                    }
                }
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

            ev.StopPropagation();
        }

        private void OnPointerLeave(PointerLeaveEvent ev)
        {
            isPointerLeaved = true;
        }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            ClearRootElementBindings();

            if (root is not null)
            {
                root.RegisterCallback<PointerDownEvent>(OnPointerDown);
                root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                root.RegisterCallback<PointerUpEvent>(OnPointerUp);
                root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

                pointerDownRegistration = CCDisposable.CreateLight(
                    (@this: this, root),
                    static (args) =>
                    {
                        var (@this, root) = args;
                        root.UnregisterCallback<PointerDownEvent>(@this.OnPointerDown);
                    });

                pointerMoveRegistration = CCDisposable.CreateLight(
                    (@this: this, root),
                    static (args) =>
                    {
                        var (@this, root) = args;
                        root.UnregisterCallback<PointerMoveEvent>(@this.OnPointerMove);
                    });

                pointerUpRegistration = CCDisposable.CreateLight(
                    (@this: this, root),
                    static (args) =>
                    {
                        var (@this, root) = args;
                        root.UnregisterCallback<PointerUpEvent>(@this.OnPointerUp);
                    });

                pointerLeaveRegistration = CCDisposable.CreateLight(
                    (@this: this, root),
                    static (args) =>
                    {
                        var (@this, root) = args;
                        root.UnregisterCallback<PointerLeaveEvent>(@this.OnPointerLeave);
                    });
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
    }
}
