using CCEnvs.Disposables;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class DropHandler
        :
        CCBehaviour,
        IDropHandler
    {
        [SerializeField]
        protected string? targetName;

        private LightDisposable<IEventHandler> registryHandle;

        private ReactiveCommand<DropContext>? onDropCmd;

        [field: GetBySelf]
        public PanelRenderer renderer { get; private set; } = null!;

        public VisualElement? root { get; private set; }
        public VisualElement? target { get; private set; }

        public string? TargetName {
            get => targetName;
            set => SetTargetName(value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BindUIReload();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnbindUIReload();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onDropCmd?.Dispose();
        }

        public DropHandler SetTargetName(string? name)
        {
            targetName = name;
            return this;
        }

        public void SendDropEvent<TEvent>(DragContext<TEvent> dragContext)
            where TEvent : EventBase<TEvent>, new()
        {
            OnDropEvent();
            var context = DropContext.Create(dragContext, gameObject);
            onDropCmd?.Execute(context);
        }

        public Observable<DropContext> ObserveDrop()
        {
            onDropCmd ??= new ReactiveCommand<DropContext>();
            return onDropCmd;
        }

        protected virtual void OnDropEvent() { }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            this.root = root;

            if (targetName.IsNullOrWhiteSpace())
                target = root;
            else
                target = root.Q<VisualElement>(targetName);

            if (target is not null)
                registryHandle = DropTargetRegistry.Register(target, gameObject);
        }

        private void BindUIReload() 
        {
            renderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void UnbindUIReload()
        {
            registryHandle.Dispose();
            renderer.UnregisterUIReloadCallback(OnUIReload);
            root = null;
            target = null;
        }
    }
}
