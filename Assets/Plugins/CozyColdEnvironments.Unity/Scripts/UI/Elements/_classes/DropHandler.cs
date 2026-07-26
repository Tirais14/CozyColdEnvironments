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

        public event Action<DropEvent>? OnDrop;

        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }
        public VisualElement? Target { get; private set; }

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
            Renderer.RegisterUIReloadCallback(OnUIReloadInternal);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            registryHandle.Dispose();
            Renderer.UnregisterUIReloadCallback(OnUIReloadInternal);
            RendererRoot = null;
            Target = null;
        }

        public DropHandler SetTargetName(string? name)
        {
            targetName = name;
            return this;
        }

        public void Refresh()
        {
            enabled = !enabled;
            enabled = !enabled;
        }

        public void SendDropEvent(DragEvent dragEv)
        {
            var ev = new DropEvent(
                dragEv.Source,
                dragEv.Target,
                dragEv.SourceGameObject,
                dragEv.TargetGameObject
                );

            try
            {
                OnDrop?.Invoke(ev);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnDropEvent(ev);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        protected virtual void OnDropEvent(DropEvent ev) { }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        private void OnUIReloadInternal(PanelRenderer renderer, VisualElement root)
        {
            RendererRoot = root;

            if (targetName.IsNullOrWhiteSpace())
                Target = root;
            else
                Target = root.Q<VisualElement>(targetName);

            if (Target is not null)
                registryHandle = DropTargetRegistry.Register(Target, gameObject);

            try
            {
                OnUIReload(renderer, root);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }
    }
}
