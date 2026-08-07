using CCEnvs.Disposables;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus.Elements
{
    public abstract class ContextMenuItemView<TViewModel> : View<TViewModel>
        where TViewModel : IContextMenuItemViewModel
    {
        [SerializeField]
        protected string nameElementName;

        private IDisposable? rootElementBinding;
        private IDisposable? nameBinding;

        public string NameElementName {
            get => nameElementName;
            set => SetNameElementName(value);
        }

        protected Label? NameElement { get; private set; }

        protected override void Start()
        {
            base.Start();
            rootElementBinding = ElementShowable.ObserveRootElement()
                .Subscribe(OnRootElementChangedInternal);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref rootElementBinding);
        }

        public ContextMenuItemView<TViewModel> SetNameElementName(string value)
        {
            Guard.IsNotNullOrWhiteSpace(value);
            nameElementName = value;
            return this;
        }

        protected override void OnSetViewModel(TViewModel? viewModel)
        {
            CCDisposable.Dispose(ref nameBinding);
        }

        protected override void InitViewModel(TViewModel viewModel)
        {
            BindName(viewModel);
        }

        protected virtual void OnRootElementChanged(VisualElement? root) { }

        protected virtual void OnNameChanged(string name) { }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            if (root is not null)
                NameElement = root.Q<Label>(nameElementName);

            OnRootElementChanged(root);
        }

        private void OnNameChangedInternal(string name)
        {
            if (NameElement is null)
                return;

            NameElement.text = name;

            OnNameChanged(name);
        }

        private void BindName(TViewModel viewModel)
        {
            nameBinding = viewModel.Name.Subscribe(OnNameChangedInternal);
        }
    }
}
