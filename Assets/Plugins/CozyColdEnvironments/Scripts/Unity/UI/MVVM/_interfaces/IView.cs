#nullable enable
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IView
    {
        IViewModel GetViewModel();

        object GetModel();
    }
    public interface IView<out T> : IView
        where T : IViewModel
    {
        new T GetViewModel();

        IViewModel IView.GetViewModel() => GetViewModel();
    }
    public interface IView<out TViewModel, out TModel> : IView
    where TViewModel : IViewModel
    {
        new TViewModel GetViewModel();

        new TModel GetModel();

        IViewModel IView.GetViewModel() => GetViewModel();

        object IView.GetModel() => GetModel()!;
    }
}
