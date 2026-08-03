using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Pools;
using CCEnvs.Threading;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
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
        [SerializeField, Range(1f, 3f)]
        protected int clickCountThreshold = 1;

        [SerializeField, Range(0f, 2f)]
        protected float dragTimeThreshold = 0.5f;
        [SerializeField, Range(0f, 2f)]
        protected float clickTimeThreshold = 0.1f;

        private readonly BeginDragEvent beginDragEv = new();
        private readonly DragEvent dragEv = new();
        private readonly EndDragEvent endDragEv = new();
        private readonly PointerEventSnapshot pointerDownEventSnapshot = new();

        private bool hasDragEv;

        private int clickCount;

        [GetBySelf]
        private IShowableElement showable = null!;

        private IDragPredicate? predicate;

        private IDisposable? dragTimerHandle;
        private IDisposable? rootElementBinding;
        private LightDisposable<(DragHandler, VisualElement)> pointerDownRegistration;
        private LightDisposable<(DragHandler, VisualElement)> pointerMoveRegistration;
        private LightDisposable<(DragHandler, VisualElement)> pointerUpRegistration;
        private LightDisposable<(DragHandler, VisualElement)> pointerLeaveRegistration;

        public event Action<BeginDragEvent>? OnBeginDrag;
        public event Action<DragEvent>? OnDrag;
        public event Action<EndDragEvent>? OnEndDrag;

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
            CCDisposable.Dispose(ref dragTimerHandle);
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

        protected virtual void OnBeginDragEvent(BeginDragEvent ev) { }

        protected virtual void OnDragEvent(DragEvent ev) { }

        protected virtual void OnEndDragEvent(EndDragEvent ev) { }

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

        private void ProcessPointerDown(
            PointerDownEvent pointerEv,
            VisualElement root
            )
        {
            element.RootElement.CapturePointer(pointerEv.pointerId);
            beginDragEv.SetSource(root, gameObject)
                .SetTarget(root, gameObject)
                .SetInfo(pointerDownEventSnapshot);

            IsDragging = true;

            try
            {
                OnBeginDragEvent(beginDragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnBeginDrag?.Invoke(beginDragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            if (CCDebug<DragHandler>.IsEnabled)
                this.PrintLog("Begin Drag");
        }

        private void OnPointerDown(PointerDownEvent pointerEv)
        {
            hasDragEv = false;

            if (!enabled ||
                pointerEv.button != pointerButton ||
                element.RootElement is null ||
                (!(Predicate?.Evaluate() ?? true)))
            {
                return;
            }

            clickCount++;

            if (clickCount < clickCountThreshold)
                return;

            pointerDownEventSnapshot.CaptureFrom(pointerEv);

            if (dragTimeThreshold > 0f)
            {
                dragTimerHandle = Observable.Timer(
                    ((double)dragTimeThreshold).Seconds(),
                    UnityTimeProvider.PreUpdate,
                    destroyCancellationToken
                    )
                    .Take(1)
                    .Subscribe(
                    (@this: this, pointerEv, element.RootElement),
                    static (_, args) =>
                    {
                        var (@this, pointerEV, root) = args;
                        @this.ProcessPointerDown(pointerEV, root);
                    });
            }
            else
                ProcessPointerDown(pointerEv, element.RootElement);
        }

        private void OnPointerMove(PointerMoveEvent ev)
        {
            if (!IsDragging || element.RootElement is null)
                return;

            dragEv.SetSource(element.RootElement, gameObject)
                .SetTarget(beginDragEv.Target, beginDragEv.TargetGameObject)
                .SetInfo(ev);
            hasDragEv = true;

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
            CCDisposable.Dispose(ref dragTimerHandle);

            if (!IsDragging ||
                element.RootElement is null ||
                dragEv is null)
            {
                return;
            }

            VisualElement? target = hasDragEv ? dragEv.Target : beginDragEv.Target;
            GameObject? targetGameObject = hasDragEv ? dragEv.TargetGameObject : beginDragEv.TargetGameObject;

            endDragEv.SetSource(element.RootElement, gameObject)
                .SetTarget(target, targetGameObject)
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
                            var dropEv = new DropEvent(
                                endDragEv.Source,
                                endDragEv.Target,
                                endDragEv.SourceGameObject,
                                endDragEv.TargetGameObject
                                );

                            try
                            {
                                targetDropHandler.SendDropEvent(dropEv);
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
                OnEndDragEvent(endDragEv);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnEndDrag?.Invoke(endDragEv);
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
            CCDisposable.Dispose(ref dragTimerHandle);
            clickCount = 0;
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
