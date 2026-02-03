#nullable enable
#pragma warning disable IDE1006
using CCEnvs.FuncLanguage;
using System;

namespace CCEnvs.Unity.UI
{
    public interface IView : IShowable
    {
        Maybe<IViewModel> viewModel { get; }
        Maybe<object> model { get; }

        void SetViewModel(object viewModel);

        void SetViewModelFactory(Func<object> factory);

        T GetModel<T>();
    }
    public interface IView<TViewModel> : IView
        where TViewModel : IViewModel
    {
        new Maybe<TViewModel> viewModel { get; }

        Maybe<IViewModel> IView.viewModel => viewModel.Raw;

        void SetViewModel(TViewModel viewModel);

        void SetViewModelFactory(Func<TViewModel> factory);

        void IView.SetViewModel(object viewModel)
        {
            SetViewModel(viewModel.To<TViewModel>());
        }

        void IView.SetViewModelFactory(Func<object> factory)
        {
            SetViewModelFactory(() => factory().To<TViewModel>());
        }
    }
}
