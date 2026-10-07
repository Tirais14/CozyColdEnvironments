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
        ViewElement<TViewModel>

        where TViewModel : IItemContainerViewModel
    {
        [Header("Container Settings")]
        [Space(5f)]

        [SerializeField]
        [Tooltip("Element must be Image or Button type")]
        protected string? iconViewName = "icon";
        [SerializeField]
        [Tooltip("Element must be Label type")]
        protected string? counterViewName = "counter";

        private IDisposable? iconBinding;
        private IDisposable? countBinding;

        public string? IconViewName {
            get => iconViewName;
            set => iconViewName = value;
        }

        public string? CounterViewName {
            get => counterViewName;
            set => counterViewName = value;
        }

        public VisualElement? IconView { get; private set; }

        public Label? CounterView { get; private set; }

        protected override void OnRootElementReset()
        {
            base.OnRootElementReset();
            IconView = null;
            CounterView = null;
        }

        protected override void InitRootElement(VisualElement root)
        {
            if (iconViewName.IsNotNullOrWhiteSpace())
            {
                IconView = root.Q<VisualElement>(iconViewName);

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

        protected override void OnSetViewModel(TViewModel? viewModel)
        {
            CCDisposable.Dispose(ref iconBinding);
            CCDisposable.Dispose(ref countBinding);
        }

        protected override void InitViewModel(TViewModel viewModel)
        {
            BindIcon(viewModel);
            BindCount(viewModel);
        }

        protected virtual void OnIconChanged(Sprite icon)
        {
            if (IconView is null)
                return;

            switch (IconView)
            {
                case Button button:
                    button.iconImage = Background.FromSprite(icon);
                    break;
                case Image image:
                    image.sprite = icon;
                    break;
                default:
                    throw CC.ThrowHelper.InvalidOperationException(IconView);
            }

        }
        private void BindIcon(TViewModel viewModel)
        {
            iconBinding = viewModel.Icon.Subscribe(OnIconChanged);
        }

        protected virtual void OnCountChanged(string count)
        {
            if (CounterView is null)
                return;

            CounterView.text = count;
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
