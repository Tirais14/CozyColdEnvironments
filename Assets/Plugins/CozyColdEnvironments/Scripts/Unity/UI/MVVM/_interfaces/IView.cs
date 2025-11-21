#nullable enable
#pragma warning disable IDE1006
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.UI.MVVM
{
    public interface IView
    {
        IViewModel viewModel { get; }
        object model { get; }
        bool IsMutable { get; }

        void SetViewModelUnsafe(object viewModel);

        Maybe<object> SetModelUnsafe(object model);
    }
    public interface IView<TViewModel> : IView
        where TViewModel : IViewModel
    {
        new TViewModel viewModel { get; }

        IViewModel IView.viewModel => viewModel;

        void SetViewModelUnsafe(TViewModel viewModel);

        void IView.SetViewModelUnsafe(object viewModel)
        {
            SetViewModelUnsafe(viewModel.As<TViewModel>());
        }
    }
    public interface IView<TViewModel, TModel> : IView<TViewModel>
        where TViewModel : IViewModel
    {
        new TModel model { get; }

        IViewModel IView.viewModel => viewModel;
        object IView.model => model!;

        Maybe<TModel> SetModelUnsafe(TModel model);

        Maybe<object> IView.SetModelUnsafe(object model) => SetModelUnsafe(model.As<TModel>()).Raw;
    }
}
