#nullable enable
using UTIRLib.Disposables;

namespace UTIRLib.UI.MVVM
{
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
