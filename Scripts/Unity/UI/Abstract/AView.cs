#nullable enable 
namespace UTIRLib.UI.MVVM
{
    public abstract class AView<T> : MonoX, IView
        where T : IViewModel
    {
        private LazyX<T> viewModelLazy = null!;

        protected T viewModel => viewModelLazy.Value;

        protected override void OnAwake()
        {
            base.OnAwake();

            viewModelLazy ??= new LazyX<T>(CreateViewModel);
        }

        protected abstract T CreateViewModel();

        public IViewModel GetViewModel()
        {
            viewModelLazy ??= new LazyX<T>(CreateViewModel);

            return viewModel;
        }
    }
}
