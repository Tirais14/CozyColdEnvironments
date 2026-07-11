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
        [Header("Container Settings")]
        [Space(5f)]

        [SerializeField]
        protected string? iconElementName;
        [SerializeField]
        protected string? counterElementName;

        [GetBySelf]
        protected PanelRenderer renderer = null!;

        public PanelRenderer Renderer => renderer;

        public VisualElement? RendererRoot { get; private set; }

        public string? IconElementName {
            get => iconElementName;
            set => SetIconElementName(value);
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

        public ItemContainerView<TViewModel> SetIconElementName(string? value)
        {
            iconElementName = value;
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

            if (iconElementName.IsNotNullOrWhiteSpace())
            {
                var iconView = root.Q<Image>(iconElementName);

                iconView.dataSource = GuardedViewModel;
                iconView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Icon));
            }

            if (counterElementName.IsNotNullOrWhiteSpace())
            {
                var counterView = root.Q<Label>(counterElementName);

                counterView.dataSource = GuardedViewModel;
                counterView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Count));
            }
        }
    }
}
