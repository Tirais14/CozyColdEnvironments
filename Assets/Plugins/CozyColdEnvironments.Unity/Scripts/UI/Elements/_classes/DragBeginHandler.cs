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
    public class DragBeginHandler<TDownEvent>
        :
        CCBehaviour, 
        IDragBeginHandler<TDownEvent>

        where TDownEvent : EventBase<TDownEvent>, new()
    {
        [SerializeField]
        protected string? targetName;

        private ReactiveCommand<DragAndDropContext<TDownEvent>>? onBeginDragCmd;

        public event Action<DragAndDropContext<TDownEvent>>? OnBeginDrag;

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
            UnbindDownEvent();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onBeginDragCmd?.Dispose();
        }

        public Observable<DragAndDropContext<TDownEvent>> ObserveBeginDrag()
        {
            onBeginDragCmd ??= new ReactiveCommand<DragAndDropContext<TDownEvent>>();
            return onBeginDragCmd;
        }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        protected virtual void OnDownEvent(TDownEvent ev) { }

        private void OnUIReloadInternal(PanelRenderer renderer, VisualElement root)
        {
            this.root = root;

            if (targetName.IsNullOrWhiteSpace())
                target = root;
            else
                target = root.Q<VisualElement>(targetName);

            if (target is not null)
                BindDownEvent(root);
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

        private void OnDownEventInternal(TDownEvent ev)
        {
            Guard.IsNotNull(target);

            var context = DragAndDropContext.Create(
                target,
                gameObject,
                ev
                );

            OnBeginDrag?.Invoke(context);
            onBeginDragCmd?.Execute(context);
        }

        private void BindDownEvent(VisualElement target)
        {
            target.RegisterCallback<TDownEvent>(OnDownEventInternal);
        }

        private void UnbindDownEvent()
        {
            target?.UnregisterCallback<TDownEvent>(OnDownEventInternal);
        }
    }
}
