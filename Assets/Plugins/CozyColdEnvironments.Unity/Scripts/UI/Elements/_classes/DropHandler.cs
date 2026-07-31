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

        protected virtual void OnRootElementChanged(RootElementChangedEvent root) { }

        private void OnRootElementChangedInternal(RootElementChangedEvent root)
        {
            if (root.Previous is not null)
                registryHandle.Dispose();

            if (root.Current is not null)
                registryHandle = DropTargetRegistry.Register(root.Current, gameObject);

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
