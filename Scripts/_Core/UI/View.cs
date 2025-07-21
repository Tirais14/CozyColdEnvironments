#nullable enable
namespace UTIRLib.UI
{
    public abstract class View : MonoX, IView
    {
        protected IViewModel viewModel;

        public IViewModel GetViewModel() => viewModel;
    }

    public abstract class View<T> : MonoX, IView<T>
        where T : IViewModel
    {
        protected T viewModel;

        public T GetViewModel() => viewModel;

        IViewModel IView.GetViewModel() => viewModel;
    }
}
