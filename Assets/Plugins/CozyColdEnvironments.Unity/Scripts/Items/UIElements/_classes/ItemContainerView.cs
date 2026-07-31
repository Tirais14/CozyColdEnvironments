using CCEnvs.Disposables;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.UI;
using CCEnvs.UnityX.UI.Elements;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    [DisallowMultipleComponent]
    public abstract class ItemContainerView<TViewModel>
        :
        View<TViewModel>,
        IVisualTreeElement

        where TViewModel : IItemContainerViewModel
    {
        [Header("Container Settings")]
        [Space(5f)]

        [SerializeField]
        protected VisualTreeAsset visualTree = null!;

        [SerializeField]
        [Tooltip("Element must be Image type")]
        protected string? iconViewName = "icon";
        [SerializeField]
        [Tooltip("Element must be Label type")]
        protected string? counterViewName = "counter";

        private IDisposable? iconBinding;
        private IDisposable? countBinding;
        private IDisposable? rootElementBinding;

        public string? IconViewName {
            get => iconViewName;
            set => SetIconElementName(value);
        }

        public string? CounterViewName {
            get => counterViewName;
            set => SetCounterElementName(value);
        }

        public VisualTreeAsset VisualTree {
            get => visualTree;
            set => SetVisualTree(value);
        }

        public Image? IconView { get; private set; }

        public Label? CounterView { get; private set; }

        [field: GetBySelf]
        protected IElement Element { get; private set; } = null!;

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

        public ItemContainerView<TViewModel> SetVisualTree(VisualTreeAsset value)
        {
            CC.Guard.IsNotNull(value);
            visualTree = value;
            return this;
        }

        protected override void OnSetViewModel(TViewModel? vm)
        {
            if (vm.IsNull())
            {
                CCDisposable.Dispose(ref iconBinding);
                CCDisposable.Dispose(ref countBinding);
                CCDisposable.Dispose(ref rootElementBinding);
            }
            else
                rootElementBinding = Element.ObserveRootElement().Subscribe(OnRootElementChanged);
        }

        protected override void InitViewModel(TViewModel vm) { }

        private void OnRootElementChanged(RootElementChangedEvent root)
        {
            if (root.Previous is not null)
            {
                CCDisposable.Dispose(ref iconBinding);
                CCDisposable.Dispose(ref countBinding);
                IconView = null;
                CounterView = null;
            }

            if (root.Current is not null)
            {
                if (iconViewName.IsNotNullOrWhiteSpace())
                {
                    IconView = root.Current.Q<Image>(iconViewName);

                    if (IconView is not null)
                        BindIcon(GuardedViewModel);
                }

                if (counterViewName.IsNotNullOrWhiteSpace())
                {
                    CounterView = root.Current.Q<Label>(counterViewName);

                    if (CounterView is not null)
                        BindCount(GuardedViewModel);
                }
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

    public class ItemContainerView : ItemContainerView<ItemContainerViewModel>
    {
        [SerializeField]
        protected CompareAction<int> showCounterViewPredicate;

        public CompareAction<int> ShowCounterViewPredicate {
            get => showCounterViewPredicate;
            set => SetShowCounterViewPredicate(value);
        }

        public ItemContainerView SetShowCounterViewPredicate(CompareAction<int> predicate)
        {
            showCounterViewPredicate = predicate;
            return this;
        }

        protected override ItemContainerViewModel? CreateViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
