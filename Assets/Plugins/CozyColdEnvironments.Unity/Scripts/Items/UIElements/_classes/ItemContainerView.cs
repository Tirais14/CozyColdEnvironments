using CCEnvs.UnityX.Injections;
using CCEnvs.UnityX.UI;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class ItemContainerView<TViewModel>
        :
        View<TViewModel>,
        IItemContainerView

        where TViewModel : IItemContainerViewModel
    {
        [GetBySelf]
        protected PanelRenderer renderer = null!;

        public PanelRenderer Renderer => renderer;

        public VisualElement? RendererRoot { get; private set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            renderer.UnregisterUIReloadCallback(OnUIReload);
        }

        protected override void OnSetViewModel(TViewModel? vm)
        {
            if (vm is null)
            {
                renderer.UnregisterUIReloadCallback(OnUIReload);
                RendererRoot = null;
            }
            else
                renderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            RendererRoot = root;
            var iconView = root.Q<Image>("icon");

            iconView.dataSource = GuardedViewModel;
            iconView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Icon));

            var counterView = root.Q<Label>("counter");

            counterView.dataSource = GuardedViewModel;
            counterView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Count));
        }
    }
}
