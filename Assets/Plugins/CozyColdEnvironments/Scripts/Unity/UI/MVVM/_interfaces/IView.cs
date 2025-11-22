#nullable enable
#pragma warning disable IDE1006
using CCEnvs.FuncLanguage;
using System;

namespace CCEnvs.Unity.UI
{
    public interface IView
    {
        Maybe<IViewModel> viewModel { get; }
        Maybe<object> model { get; }
        IViewModel viewModelUnsafe { get; }
        object modelUnsafe { get; }
        bool IsMutable { get; }

        void SetViewModelUnsafe(object viewModel);

        void SetViewModelFactoryUnsafe(Func<object> factory);

        Maybe<T> GetModel<T>();
        T GetModelUnsafe<T>();
    }
    public interface IView<TViewModel> : IView
        where TViewModel : IViewModel
    {
        new Maybe<TViewModel> viewModel { get; }
        new TViewModel viewModelUnsafe { get; }

        Maybe<IViewModel> IView.viewModel => viewModel.Raw;
        IViewModel IView.viewModelUnsafe => viewModelUnsafe;

        void SetViewModelUnsafe(TViewModel viewModel);

        void SetViewModelFactoryUnsafe(Func<TViewModel> factory);

        void IView.SetViewModelUnsafe(object viewModel)
        {
            SetViewModelUnsafe(viewModel.As<TViewModel>());
        }

        void IView.SetViewModelFactoryUnsafe(Func<object> factory)
        {
            SetViewModelFactoryUnsafe(() => factory().As<TViewModel>());
        }
    }
}
