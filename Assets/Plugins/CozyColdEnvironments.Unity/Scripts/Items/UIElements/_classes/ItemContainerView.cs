using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.UI;
using CommunityToolkit.Diagnostics;
using R3;
using System;
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
        protected string? iconViewName = "icon";
        [SerializeField]
        [Tooltip("Element must be Label type")]
        protected string? counterViewName = "counter";

        private IDisposable? iconBinding;
        private IDisposable? countBinding;

        public string? IconViewName {
            get => iconViewName;
            set => SetIconElementName(value);
        }

        public string? CounterViewName {
            get => counterViewName;
            set => SetCounterElementName(value);
        }

        public Image? IconView { get; private set; }

        public Label? CounterView { get; private set; }

        public ItemContainerView<TViewModel> SetIconElementName(string? value)
        {
            iconViewName = value;
            return this;
        }

        public ItemContainerView<TViewModel> SetCounterElementName(string? value)
        {
            counterViewName = value;
            return this;
        }

        protected override void OnSetViewModel(TViewModel? vm)
        {
            if (vm.IsNull())
                ElementShowable.renderer.UnregisterUIReloadCallback(OnUIReload);
            else
                ElementShowable.renderer.RegisterUIReloadCallback(OnUIReload);

            CCDisposable.Dispose(ref iconBinding);
            CCDisposable.Dispose(ref countBinding);
        }

        protected override void InitViewModel(TViewModel vm) { }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            if (iconViewName.IsNotNullOrWhiteSpace())
            {
                IconView = root.Q<Image>(iconViewName);

                if (IconView is not null)
                    BindIcon(GuardedViewModel);
            }

            if (counterViewName.IsNotNullOrWhiteSpace())
            {
                CounterView = root.Q<Label>(counterViewName);

                if (CounterView is not null)
                    BindCount(GuardedViewModel);
            }
        }

        protected virtual void OnIconChanged(Sprite icon)
        {
            if (IconView is null)
                return;

            IconView.sprite = icon;
        }

        protected virtual void OnCountChanged(string count)
        {
            if (CounterView is null)
                return;

            CounterView.text = count;
        }

        private void BindIcon(TViewModel viewModel)
        {
            iconBinding = viewModel.Icon.Subscribe(OnIconChanged);
        }

        private void BindCount(TViewModel viewModel)
        {
            countBinding = viewModel.Count.Subscribe(OnCountChanged);
        }

    }
}
