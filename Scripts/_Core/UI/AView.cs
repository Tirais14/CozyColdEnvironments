#nullable enable
namespace UTIRLib.UI
{
    public abstract class AView : MonoX, IView
    {
        protected IViewModel viewModel;

        public IViewModel GetViewModel() => viewModel;
    }

    public abstract class AView<T> : MonoX, IView<T>
        where T : IViewModel
    {
        protected T viewModel;

        public T GetViewModel() => viewModel;

        IViewModel IView.GetViewModel() => viewModel;
    }
}
