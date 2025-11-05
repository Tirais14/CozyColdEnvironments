using CCEnvs.Reflection;
using CCEnvs.Unity.UI.Elements;
using System;
using UniRx;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236
#pragma warning disable S1117
namespace CCEnvs.Unity.UI.MVVM
{
    /// <summary>
    /// <see cref="TModel"/> must have empty constructor by default
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class View<TViewModel, TModel>
        : ViewElement,
        IView<TViewModel, TModel>

        where TViewModel : ViewModel<TModel>
    {
        protected Lazy<TViewModel> _viewModel;

        public TModel model => viewModel.model;
        public TViewModel viewModel => _viewModel.Value;

        protected override void Awake()
        {
            base.Awake();

            _viewModel = new Lazy<TViewModel>(() => CreateViewModel().AddTo(this));
        }

        protected virtual TModel CreateModel()
        {
            var model = typeof(TModel).ReflectQuery()
                                      .NonPublic()
                                      .Invoke<TModel>();

            if (model is IDisposable disposable)
                disposable.AddTo(this);

            return model;
        }

        protected virtual TViewModel CreateViewModel(TModel? model = default)
        {
            model ??= CreateModel();

            return typeof(TViewModel).ReflectQuery()
                                     .NonPublic()
                                     .Arguments(model!)
                                     .Invoke<TViewModel>();
        }

        public override void Show()
        {
            base.Show();

            viewModel.ForceNotify();
        }
    }
}
