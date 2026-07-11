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

        [SerializeField]
        protected string? imageElementName;
        [SerializeField]
        protected string? counterElementName;

        public PanelRenderer Renderer => renderer;

        public VisualElement? RendererRoot { get; private set; }

        public string? ImageElementName {
            get => imageElementName;
            set => SetImageElementName(value);
        }

        public string? CounterElementName {
            get => counterElementName;
            set => SetCounterElementName(value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            renderer.UnregisterUIReloadCallback(OnUIReload);
        }

        public ItemContainerView<TViewModel> SetImageElementName(string? value)
        {
            imageElementName = value;
            return this;
        }

        public ItemContainerView<TViewModel> SetCounterElementName(string? value)
        {
            counterElementName = value;
            return this;
        }

        protected override void OnSetViewModel(TViewModel? vm)
        {
            if (vm.IsNull())
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

            if (imageElementName.IsNotNullOrWhiteSpace())
            {
                var iconView = root.Q<Image>("icon");

                iconView.dataSource = GuardedViewModel;
                iconView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Icon));
            }

            if (counterElementName.IsNotNullOrWhiteSpace())
            {
                var counterView = root.Q<Label>("counter");

                counterView.dataSource = GuardedViewModel;
                counterView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Count));
            }
        }
    }
}
