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
    [DisallowMultipleComponent]
    public class DropHandler
        :
        CCBehaviour,
        IDropHandler
    {
        private LightDisposable<IEventHandler> registryHandle;

        private IDisposable? rootElementBinding;

        public event Action<DropEvent>? OnDrop;

        [field: GetBySelf]
        protected IElement element { get; private set; } = null!;

        bool IToggleable.IsEnabled {
            get => enabled;
            set => enabled = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            rootElementBinding = element.ObserveRootElement().Subscribe(OnRootElementChangedInternal);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CCDisposable.Dispose(ref rootElementBinding);
        }

        public void SendDropEvent(DropEvent ev)
        {
            try
            {
                OnDropEvent(ev);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            try
            {
                OnDrop?.Invoke(ev);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        protected virtual void OnDropEvent(DropEvent ev) { }

        protected virtual void OnUIReload(PanelRenderer renderer, VisualElement root) { }

        protected virtual void OnRootElementChanged(VisualElement? root) { }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            registryHandle.Dispose();
            registryHandle = default;

            if (root is not null)
                registryHandle = DropTargetRegistry.Register(root, gameObject);

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
