#nullable enable
#pragma warning disable IDE1006
using System;

namespace CCEnvs.Unity.UI
{
    public interface IView : IShowable
    {
        IViewModel? ViewModel { get; }
        object? Model { get; }

        void SetViewModel(object viewModel);

        void SetViewModelFactory(Func<object> factory);

        T GetModel<T>();

        T GetViewModel<T>() where T : IViewModel;
    }
    public interface IView<TViewModel> : IView
        where TViewModel : IViewModel
    {
        new TViewModel? ViewModel { get; }

        IViewModel? IView.ViewModel => ViewModel;

        void SetViewModel(TViewModel viewModel);

        void SetViewModelFactory(Func<TViewModel> factory);

        void IView.SetViewModel(object viewModel)
        {
            SetViewModel(viewModel.CastTo<TViewModel>());
        }

        void IView.SetViewModelFactory(Func<object> factory)
        {
            SetViewModelFactory(() => factory().CastTo<TViewModel>());
        }
    }
}
