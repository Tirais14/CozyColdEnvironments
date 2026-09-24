using CCEnvs.Disposables;
using R3;
using System;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public abstract class ViewElement<TViewModel> : View<TViewModel>
        where TViewModel : IViewModel
    {
        private IDisposable? rootElementBinding;

        protected override void Start()
        {
            base.Start();
            BindRootElement();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref rootElementBinding);
        }

        protected virtual void OnRootElementSet(VisualElement root)
        {

        }

        protected virtual void OnRootElementReset()
        {
        }

        protected virtual void OnRootElementChanged(VisualElement? root)
        {

        }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            if (root is null)
                OnRootElementReset();
            else
                OnRootElementSet(root);

            OnRootElementChanged(root);
        }

        private void BindRootElement()
        {
            rootElementBinding = ElementShowable.ObserveRootElement()
                .Subscribe(OnRootElementChangedInternal);
        }
    }
}
