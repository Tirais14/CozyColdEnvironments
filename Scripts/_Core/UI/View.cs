#nullable enable
namespace UTIRLib.UI
{
    public abstract class View<T> : MonoX, IView
        where T : IViewModel
    {
        protected T viewModel;
    }
    public abstract class View : View<IViewModel>, IView
    {
    }
}
