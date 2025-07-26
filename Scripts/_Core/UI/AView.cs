#nullable enable
using UTIRLib.Attributes;

namespace UTIRLib.UI
{
    public abstract class AView : MonoX, IView
    {
        [RequiredField]
        protected IViewModel viewModel;

        public IViewModel GetViewModel() => viewModel;
    }

    public abstract class AView<T> : MonoX, IView<T>
        where T : IViewModel
    {
        [RequiredField]
        protected T viewModel;

        public T GetViewModel() => viewModel;

        IViewModel IView.GetViewModel() => viewModel;
    }
}
