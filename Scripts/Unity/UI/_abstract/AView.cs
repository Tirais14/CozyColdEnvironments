#nullable enable 
namespace CCEnvs.UI.MVVM
{
    public abstract class AView<T> : MonoCC, IView
        where T : IViewModel
    {
        private LazyCC<T> viewModelLazy = null!;

        protected T viewModel => viewModelLazy.Value;

        protected override void OnAwake()
        {
            base.OnAwake();

            viewModelLazy ??= new LazyCC<T>(CreateViewModel);
        }

        protected abstract T CreateViewModel();

        public IViewModel GetViewModel()
        {
            viewModelLazy ??= new LazyCC<T>(CreateViewModel);

            return viewModel;
        }
    }
}
