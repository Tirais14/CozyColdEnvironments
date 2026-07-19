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
    public abstract class DragEventHandler<TEvent> : CCBehaviour

        where TEvent : EventBase<TEvent>, new()
    {
        [SerializeField]
        protected string? targetName;

        protected event Action<DragAndDropContext<TEvent>>? OnEvent;

        private ReactiveCommand<DragAndDropContext<TEvent>>? onEventCmd;

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
            onEventCmd?.Dispose();
        }

        protected Observable<DragAndDropContext<TEvent>> ObserveEvent()
        {
            onEventCmd ??= new ReactiveCommand<DragAndDropContext<TEvent>>();
            return onEventCmd;
        }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        protected virtual void OnDownEvent(TEvent ev) { }

        protected abstract Vector3 GetEventPosition();

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

        private void OnDownEventInternal(TEvent ev)
        {
            Guard.IsNotNull(target);

            var context = DragAndDropContext.Create(
                target,
                gameObject,
                ev
                );

            OnEvent?.Invoke(context);
            onEventCmd?.Execute(context);
        }

        private void BindDownEvent(VisualElement target)
        {
            target.RegisterCallback<TEvent>(OnDownEventInternal);
        }

        private void UnbindDownEvent()
        {
            target?.UnregisterCallback<TEvent>(OnDownEventInternal);
        }
    }
}
