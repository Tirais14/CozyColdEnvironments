#nullable enable
#pragma warning disable IDE1006
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.UI.MVVM
{
    public interface IView
    {
        IPresenter viewModel { get; }
        object model { get; }
        bool IsMutable { get; }

        void SetViewModelUnsafe(object viewModel);

        Maybe<object> SetModelUnsafe(object model);
    }
    public interface IView<TViewModel> : IView
        where TViewModel : IPresenter
    {
        new TViewModel viewModel { get; }

        IPresenter IView.viewModel => viewModel;

        void SetViewModelUnsafe(TViewModel viewModel);

        void IView.SetViewModelUnsafe(object viewModel)
        {
            SetViewModelUnsafe(viewModel.As<TViewModel>());
        }
    }
    public interface IView<TViewModel, TModel> : IView<TViewModel>
        where TViewModel : IPresenter
    {
        new TModel model { get; }

        IPresenter IView.viewModel => viewModel;
        object IView.model => model!;

        Maybe<TModel> SetModelUnsafe(TModel model);

        Maybe<object> IView.SetModelUnsafe(object model) => SetModelUnsafe(model.As<TModel>()).Raw;
    }
}
