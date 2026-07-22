#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using UnityEngine;
using UnityEngine.UIElements;

namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class DragTarget : CCBehaviour, IDragTarget, IElement
    {
        private Vector2 defaultPosition;
        private StyleEnum<Position> defaultPositionType;

        private LightDisposable<IEventHandler> registryBinding;

        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }

        public int layer => gameObject.layer;

        protected override void Awake()
        {
            base.Awake();
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
            RestoreDefaultStyle();
            registryBinding.Dispose();
            RendererRoot = null;
        }

        public DragTarget SetPosition(Vector2 position)
        {
            AssertRoot();
            if (RendererRoot is not null)
            {
                RendererRoot.style.left = position.x;
                RendererRoot.style.top = position.y;
            }

            return this;
        }

        public DragTarget ResetPosition()
        {
            AssertRoot();
            if (RendererRoot is not null)
            {
                RendererRoot.style.left = defaultPosition.x;
                RendererRoot.style.top = defaultPosition.y;
            }

            return this;
        }

        private void CaptureDefaultStyle(VisualElement root)
        {
            defaultPositionType = root.style.position;
            defaultPosition = new Vector2(
                x: root.style.left.value.value,
                y: root.style.top.value.value
                );
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            this.RendererRoot = root;
            registryBinding = DragTargetRegistry.Register(root, this);
            CaptureDefaultStyle(root);
            root.style.position = Position.Absolute;
        }

        private void RestoreDefaultStyle()
        {
            if (RendererRoot is null)
                return;

            ResetPosition();
            RendererRoot.style.position = defaultPositionType;
        }

        private void BindUIReload()
        {
            Renderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void UnbindUIReload()
        {
            Renderer.UnregisterUIReloadCallback(OnUIReload);
        }

        private void AssertRoot()
        {
            if (CCDebug<DragTarget>.IsEnabled)
                this.AssertWarning(RendererRoot is not null, "root is null");
        }
    }
}
