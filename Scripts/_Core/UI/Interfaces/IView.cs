#nullable enable
namespace UTIRLib.UI
{
    public interface IView
    {
        IViewModel GetViewModel();
    }
    public interface IView<out T> : IView
        where T : IViewModel
    {
        new T GetViewModel();

        IViewModel IView.GetViewModel() => GetViewModel();
    }
}
