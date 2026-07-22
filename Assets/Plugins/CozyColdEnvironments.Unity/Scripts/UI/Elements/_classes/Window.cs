using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class Window : CCBehaviour, IElement
    {
        [field: GetBySelf]
        public PanelRenderer Renderer { get; private set; } = null!;

        public VisualElement? RendererRoot { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            Renderer.RegisterUIReloadCallback(OnUIReload);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Renderer.UnregisterUIReloadCallback(OnUIReload);
            RendererRoot = null;
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            RendererRoot = root;
        }
    }
}
