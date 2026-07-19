using CCEnvs.UnityX.Components;
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

        private ReactiveCommand<DragAndDropContext<TBeginEvent>>? onBeginDragCmd;
        private ReactiveCommand<DragAndDropContext<TDragEvent>>? onDragCmd;
        private ReactiveCommand<DragAndDropContext<TEndEvent>>? onEndDragCmd;

        [field: GetBySelf]
        public PanelRenderer renderer { get; private set; } = null!;

        public VisualElement? root { get; private set; }

        public VisualElement? target { get; private set; }

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

        public Observable<DragAndDropContext<TBeginEvent>> ObserveBeginDrag()
        {
            onBeginDragCmd ??= new ReactiveCommand<DragAndDropContext<TBeginEvent>>();
            return onBeginDragCmd;
        }

        public Observable<DragAndDropContext<TDragEvent>> ObserveDrag()
        {
            onDragCmd ??= new ReactiveCommand<DragAndDropContext<TDragEvent>>();
            return onDragCmd;
        }

        public Observable<DragAndDropContext<TEndEvent>> ObserveEndDrag()
        {
            onEndDragCmd ??= new ReactiveCommand<DragAndDropContext<TEndEvent>>();
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

            var context = DragAndDropContext.Create(
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

            var context = DragAndDropContext.Create(
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

            var context = DragAndDropContext.Create(
                target,
                gameObject,
                ev
                );

            OnEndEvent(ev);
            onEndDragCmd?.Execute(context);
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
