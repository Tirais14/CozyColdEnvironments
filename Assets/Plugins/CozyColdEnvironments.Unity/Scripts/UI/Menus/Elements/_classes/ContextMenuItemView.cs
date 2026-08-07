using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
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
        protected string nameElementName = "function";

        [SerializeField]
        protected string buttonElementName = "function";

        private IDisposable? rootElementBinding;
        private IDisposable? nameBinding;

        public string NameElementName {
            get => nameElementName;
            set
            {
                Guard.IsNotNullOrWhiteSpace(value);
                nameElementName = value;
            }
        }
        public string ButtonElementName {
            get => buttonElementName;
            set
            {
                Guard.IsNotNullOrWhiteSpace(value);
                buttonElementName = value;
            }
        }

        protected TextElement? NameElement { get; private set; }

        protected Button? ButtonElement { get; private set; }

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

            if (ButtonElement is not null)
                ButtonElement.clicked -= OnButtonClickedInternal;
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

        protected virtual void OnButtonClicked() { }

        private void OnButtonClickedInternal()
        {
            ViewModel.Maybe()
                .Map(x => x.Model)
                .Cast<IContextMenuItem>()
                .IfSome(model => model.Invoke());

            OnButtonClicked();
        }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            if (ButtonElement is not null)
                ButtonElement.clicked -= OnButtonClickedInternal;

            if (root is not null)
            {
                NameElement = root.Q<TextElement>(nameElementName);
                ButtonElement = root.Q<Button>(buttonElementName);

                if (ButtonElement is not null)
                    ButtonElement.clicked += OnButtonClickedInternal;
            }

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
