#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IView
    {
        IViewModel viewModel { get; }
        object model { get; }
        bool ViewModelMutable { get; }

        void SetViewModelUnsafe(object viewModel);
    }
    public interface IView<TViewModel> : IView
        where TViewModel : IViewModel
    {
        new TViewModel viewModel { get; }

        IViewModel IView.viewModel => viewModel;

        void SetViewModelUnsafe(TViewModel viewModel);

        void IView.SetViewModelUnsafe(object viewModel) => SetViewModelUnsafe(viewModel.As<TViewModel>());
    }
    public interface IView<TViewModel, TModel> : IView<TViewModel>
    where TViewModel : IViewModel
    {
        new TModel model { get; }

        IViewModel IView.viewModel => viewModel;

        object IView.model => model!;
    }
}
