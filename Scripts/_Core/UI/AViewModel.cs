#nullable enable
namespace UTIRLib.UI
{
    public abstract class AViewModel : DisposableContainer, IViewModel
    {
        protected object model;

        protected AViewModel(object model)
        {
            this.model = model;
        }

        public object GetModel() => model;
    }
    public abstract class AViewModel<T> : DisposableContainer, IViewModel<T>
    {
        protected T model;

        protected AViewModel(T model)
        {
            this.model = model;
        }

        public T GetModel() => model;
        object IViewModel.GetModel() => model!;
    }
}
