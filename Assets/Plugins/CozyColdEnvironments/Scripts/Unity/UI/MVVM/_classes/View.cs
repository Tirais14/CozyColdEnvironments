using CCEnvs.Reflection.Data;
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
        private Lazy<TViewModel> _viewModel;

        public TModel model => viewModel.model;
        public TViewModel viewModel {
            get
            {
                _viewModel ??= new Lazy<TViewModel>(CreateViewModel);

                return _viewModel.Value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _viewModel = new Lazy<TViewModel>(CreateViewModel);
        }

        protected virtual TModel CreateModel()
        {
            var model = InstanceFactory.Create<TModel>(
                parameters: InstanceFactory.Parameters.CacheConstructor
                            |
                            InstanceFactory.Parameters.ThrowIfNotFound)!;

            if (model is IDisposable disposable)
                disposable.AddTo(this);

            return model;
        }

        protected virtual TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

#if UNITY_2017_1_OR_NEWER
            return InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(
                    new ExplicitArgument(model!),
                    new ExplicitArgument(gameObject)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
#else
            return InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(
                    new ExplicitArgument(model!)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
#endif //UNITY_2017_1_OR_NEWER
        }


    }
}
