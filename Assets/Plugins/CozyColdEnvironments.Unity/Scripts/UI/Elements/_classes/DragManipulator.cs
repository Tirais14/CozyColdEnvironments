//using CCEnvs.Diagnostics;
//using CCEnvs.Disposables;
//using CCEnvs.Pools;
//using CCEnvs.UnityX.ComponentInjections;
//using CCEnvs.UnityX.UI.Elements;
//using System;
//using UnityEngine;
//using UnityEngine.UIElements;

//#nullable enable
//namespace CCEnvs.UnityX.UI.Elements
//{
//    public class DragManipulator
//        :
//        PointerManipulator
//    {
//        protected string? dropTargetTag;

//        protected LayerMask dropTargetLayerMask = ~0;

//        protected int pointerButton = 0;

//        protected float dragThreshold = 0.2f;

//        private readonly DragEvent dragEv = new();

//        private IDragPredicate? predicate;

//        private float secondsSincePointerDown;

//        private IDisposable? rootElementBinding;
//        private LightDisposable<(DragManipulator, VisualElement)> pointerDownRegistration;
//        private LightDisposable<(DragManipulator, VisualElement)> pointerMoveRegistration;
//        private LightDisposable<(DragManipulator, VisualElement)> pointerUpRegistration;

//        public event DragAction? OnBeginDrag;
//        public event DragAction? OnDrag;
//        public event DragAction? OnEndDrag;

//        public virtual bool IsDragging { get; private set; }

//        public IDragPredicate? Predicate {
//            get => predicate;
//            set => SetPredicate(value);
//        }

//        public string? DropTargetTag {
//            get => dropTargetTag;
//            set => SetDropTargetTag(value);
//        }

//        public int DropTargetLayerMask {
//            get => dropTargetLayerMask;
//            set => SetDropTargetLayerMask(value);
//        }
//        public int PointerButton {
//            get => pointerButton;
//            set => SetPointerButton(value);
//        }

//        public GameObject GameObject { get; }

//        public DragManipulator(GameObject gameObject)
//        {
//            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

//            GameObject = gameObject;
//        }

//        protected override void RegisterCallbacksOnTarget()
//        {
//            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
//            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
//            target.RegisterCallback<PointerUpEvent>(OnPointerUp);

//            pointerDownRegistration = CCDisposable.CreateLight(
//                (@this: this, target),
//                static (args) =>
//                {
//                    var (@this, root) = args;
//                    root.UnregisterCallback<PointerDownEvent>(@this.OnPointerDown);
//                });

//            pointerMoveRegistration = CCDisposable.CreateLight(
//                (@this: this, target),
//                static (args) =>
//                {
//                    var (@this, root) = args;
//                    root.UnregisterCallback<PointerMoveEvent>(@this.OnPointerMove);
//                });

//            pointerUpRegistration = CCDisposable.CreateLight(
//                (@this: this, target),
//                static (args) =>
//                {
//                    var (@this, root) = args;
//                    root.UnregisterCallback<PointerUpEvent>(@this.OnPointerUp);
//                });
//        }

//        protected override void UnregisterCallbacksFromTarget()
//        {
//            IsDragging = false;
//            CCDisposable.Dispose(ref rootElementBinding);
//            ClearRootElementBindings();
//        }

//        public DragManipulator SetDropTargetTag(string? tag)
//        {
//            dropTargetTag = tag;
//            return this;
//        }

//        public DragManipulator SetDropTargetLayerMask(int layerMask)
//        {
//            dropTargetLayerMask = layerMask;
//            return this;
//        }

//        public DragManipulator SetPredicate(IDragPredicate? value)
//        {
//            predicate = value;
//            return this;
//        }

//        public DragManipulator SetPointerButton(int value)
//        {
//            pointerButton = value;
//            return this;
//        }

//        protected virtual void OnBeginDragEvent(DragEvent ev) { }

//        protected virtual void OnDragEvent(DragEvent ev) { }

//        protected virtual void OnEndDragEvent(DragEvent ev) { }

//        protected virtual void OnRootElementChanged(VisualElement? root) { }

//        private void OnRootElementChangedInternal(VisualElement? root)
//        {
//            ClearRootElementBindings();

//            if (root is not null)
//            {

//            }

//            try
//            {
//                OnRootElementChanged(root);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }
//        }

//        private void ClearRootElementBindings()
//        {
//            pointerDownRegistration.Dispose();
//            pointerMoveRegistration.Dispose();
//            pointerUpRegistration.Dispose();

//            pointerDownRegistration = default;
//            pointerMoveRegistration = default;
//            pointerUpRegistration = default;
//        }

//        private void OnPointerDown(PointerDownEvent pointerEv)
//        {
//            if (pointerEv.button != pointerButton ||
//                (!Predicate?.Evaluate() ?? false))
//            {
//                return;
//            }

//            target.CapturePointer(pointerEv.pointerId);
//            dragEv.SetSource(target, GameObject)
//                .SetTarget(target, GameObject)
//                .SetInfo(pointerEv);

//            IsDragging = secondsSincePointerDown > dragThreshold;

//            try
//            {
//                OnBeginDragEvent(dragEv);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }

//            try
//            {
//                OnBeginDrag?.Invoke(dragEv);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }

//            if (CCDebug<DragHandler>.IsEnabled)
//                this.PrintLog("Begin Drag");
//        }

//        private void OnPointerMove(PointerMoveEvent ev)
//        {
//            if (target is null)
//                return;

//            secondsSincePointerDown += Time.unscaledDeltaTime;
//            IsDragging = secondsSincePointerDown > dragThreshold;

//            if (!IsDragging)
//                return;

//            dragEv.SetSource(target, GameObject)
//                .SetInfo(ev);

//            try
//            {
//                OnDragEvent(dragEv);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }

//            try
//            {
//                OnDrag?.Invoke(dragEv);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }
//        }

//        private void OnPointerUp(PointerUpEvent ev)
//        {
//            secondsSincePointerDown = 0f;

//            if (!IsDragging ||
//                target is null ||
//                dragEv is null)
//            {
//                return;
//            }

//            dragEv.SetSource(target, GameObject)
//                .SetInfo(ev);

//            using (var dropElements = new PooledList<VisualElement>(null))
//            {
//                if (showable.Root.IfNull(showable).RootElement.Is(out VisualElement? root))
//                {
//                    root.panel.PickAll(ev.position, dropElements);

//                    for (int i = 0; i < dropElements.Count; i++)
//                    {
//                        VisualElement dropElement = dropElements[i];

//                        if (DropTargetRegistry.Targets.TryGetValue(dropElement, out DropTarget dropTarget) &&
//                            gameObject != dropTarget.GameObject &&
//                            (dropTargetTag.IsNullOrWhiteSpace() || dropTarget.GameObject.CompareTag(dropTargetTag)) &&
//                            (DropTargetLayerMask & (1 << dropTarget.GameObject.layer)) != 0 &&
//                            dropTarget.GameObject.Q()
//                                .Component<IDropHandler>()
//                                .Lax()
//                                .TryGetValue(out var targetDropHandler)
//                            )
//                        {
//                            try
//                            {
//                                targetDropHandler.SendDropEvent(dragEv);
//                            }
//                            catch (Exception ex)
//                            {
//                                this.PrintException(ex);
//                            }

//                            break;
//                        }
//                    }
//                }
//            }


//            element.RootElement.ReleasePointer(ev.pointerId);

//            try
//            {
//                OnEndDragEvent(dragEv);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }

//            try
//            {
//                OnEndDrag?.Invoke(dragEv);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }

//            IsDragging = false;

//            if (CCDebug<DragHandler>.IsEnabled)
//                this.PrintLog("End Drag");

//            dragEv.SetTarget(null, null);
//        }
//    }
//}
