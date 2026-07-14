using CCEnvs.UnityX.Injections;
using CCEnvs.UnityX.UI;
using CommunityToolkit.Diagnostics;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class ItemContainerView<TViewModel>
        :
        View<TViewModel>

        where TViewModel : IItemContainerViewModel
    {
        [Header("Container Settings")]
        [Space(5f)]

        [SerializeField]
        [Tooltip("Element must be Image type")]
        protected string? iconElementName = "icon";
        [SerializeField]
        [Tooltip("Element must be Label type")]
        protected string? counterElementName = "counter";

        public string? IconElementName {
            get => iconElementName;
            set => SetIconElementName(value);
        }

        public string? CounterElementName {
            get => counterElementName;
            set => SetCounterElementName(value);
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
                ElementShowable.renderer.UnregisterUIReloadCallback(OnUIReload);
            else
                ElementShowable.renderer.RegisterUIReloadCallback(OnUIReload);
        }

        protected override void InitViewModel(TViewModel vm) { }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            if (iconElementName.IsNotNullOrWhiteSpace())
            {
                var iconView = root.Q<Image>(iconElementName);

                Guard.IsNotNull(iconView, nameof(iconView));

                iconView.dataSource = GuardedViewModel;
                iconView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Icon));
            }

            if (counterElementName.IsNotNullOrWhiteSpace())
            {
                var counterView = root.Q<Label>(counterElementName);

                Guard.IsNotNull(counterView, nameof(counterView));

                counterView.dataSource = GuardedViewModel;
                counterView.dataSourcePath = new PropertyPath(nameof(GuardedViewModel.Count));
            }
        }
    }
}
