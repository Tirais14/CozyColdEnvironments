#nullable enable
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IView
    {
        IViewModel GetViewModel();

        object GetModel();
    }
    public interface IView<out TViewModel> : IView
        where TViewModel : IViewModel
    {
        new TViewModel GetViewModel();

        IViewModel IView.GetViewModel() => GetViewModel();
    }
    public interface IView<out TViewModel, out TModel> : IView<TViewModel>
    where TViewModel : IViewModel
    {
        new TModel GetModel();

        IViewModel IView.GetViewModel() => GetViewModel();

        object IView.GetModel() => GetModel()!;
    }
}
