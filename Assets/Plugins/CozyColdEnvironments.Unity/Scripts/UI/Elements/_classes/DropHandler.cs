using CCEnvs.Disposables;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.ComponentInjections;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class DropHandler
        :
        CCBehaviour,
        IDropHandler,
        IElement
    {
        [SerializeField]
        protected string? targetName;

        private LightDisposable<IEventHandler> registryHandle;

        private ReactiveCommand<DropContext>? onDropCmd;

        public event Action<DropContext>? OnDrop;

        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }
        public VisualElement? target { get; private set; }

        public string? TargetName {
            get => targetName;
            set => SetTargetName(value);
        }

        bool IToggleable.IsEnabled {
            get => enabled;
            set => enabled = value;
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
            CC.LogHelper.AssertMonoBehaviourStarted(this);
            targetName = name;
            return this;
        }

        public void SendDropEvent(DragContext dragContext)
        {
            OnDropEvent();
            var context = DropContext.Create(dragContext, gameObject);
            OnDrop?.Invoke(context);
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
            this.RendererRoot = root;

            if (targetName.IsNullOrWhiteSpace())
                target = root;
            else
                target = root.Q<VisualElement>(targetName);

            if (target is not null)
                registryHandle = DropTargetRegistry.Register(target, gameObject);
        }

        private void BindUIReload() 
        {
            Renderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void UnbindUIReload()
        {
            registryHandle.Dispose();
            Renderer.UnregisterUIReloadCallback(OnUIReload);
            RendererRoot = null;
            target = null;
        }
    }
}
