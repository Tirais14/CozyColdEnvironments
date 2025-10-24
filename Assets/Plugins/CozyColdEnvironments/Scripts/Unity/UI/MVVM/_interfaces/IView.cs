#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IView
    {
        IViewModel viewModel { get; }

        object model { get; }
    }
    public interface IView<out TViewModel> : IView
        where TViewModel : IViewModel
    {
        new TViewModel viewModel { get; }

        IViewModel IView.viewModel => viewModel;
    }
    public interface IView<out TViewModel, out TModel> : IView<TViewModel>
    where TViewModel : IViewModel
    {
        new TModel model { get; }

        IViewModel IView.viewModel => viewModel;

        object IView.model => model!;
    }
}
