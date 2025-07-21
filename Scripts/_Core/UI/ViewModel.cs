#nullable enable
#pragma warning disable S3881
namespace UTIRLib.UI
{
    public abstract class ViewModel : ViewModelBase, IViewModel
    {
        protected object model;

        protected ViewModel(object model)
        {
            this.model = model;
        }

        public object GetModel() => model;
    }
    public abstract class ViewModel<T> : ViewModelBase, IViewModel<T>
    {
        protected T model;

        protected ViewModel(T model)
        {
            this.model = model;
        }

        public T GetModel() => model;
        object IViewModel.GetModel() => model!;
    }
}
