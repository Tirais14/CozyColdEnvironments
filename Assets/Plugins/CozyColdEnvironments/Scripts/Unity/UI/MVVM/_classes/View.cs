using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.UI.Elements;
using System;
using UniRx;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
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

        [SerializeField]
        protected bool isMutable;

        public TModel model => viewModel.model;
        public TViewModel viewModel => _viewModel.Value;
        public bool IsMutable => isMutable;

        protected override void Awake()
        {
            base.Awake();

            _viewModel = new Lazy<TViewModel>(() => ViewModelFactory());
        }

        protected override void Start()
        {
            base.Start();

            viewModel.Maybe().IfSome(_ => InstallBingings());
        }

        public virtual void SetViewModelUnsafe(TViewModel viewModel)
        {
            if (!IsMutable)
                throw new InvalidOperationException("Cannot set new view model.");
            CC.Guard.IsNotNull(viewModel, nameof(viewModel));

            this.viewModel.Maybe().IfSome(viewModel => viewModel.Dispose());

            _viewModel = new Lazy<TViewModel>(viewModel);

            if (viewModel is IDisposable disp)
                disp.AddTo(this);

            InstallBingings();
        }

        public Maybe<TModel> SetModelUnsafe(TModel model)
        {
            if (!IsMutable)
                throw new InvalidOperationException("Cannot set new model.");

            var tModel = viewModel.Maybe().Map(viewModel => viewModel.model).Raw;

            SetViewModelUnsafe(ViewModelFactory(model));

            return tModel;
        }

        protected virtual TModel ModelFactory()
        {
            Type modelType = typeof(TModel);

            if (modelType.IsType<Component>())
                throw new InvalidOperationException($"View not contain model. Unity objects must be instantiated by Unity instead of creating new instance. Model: {modelType.GetFullName()}.");

            TModel model = typeof(TModel).Reflect().Cache().CreateInstance<TModel>();

            if (model is IDisposable disposable)
                disposable.AddTo(this);

            return model;
        }

        protected virtual TViewModel ViewModelFactory(TModel? model = default)
        {
            model = model.Maybe()
                         .Match(
                some: m => m,
                none: () => ModelFactory())
                         .Raw;

            if (model.IsNull())
                return null!;

            return typeof(TViewModel).Reflect()
                                     .Cache()            
                                     .Arguments(model!, gameObject)
                                     .CreateInstance<TViewModel>();
        }

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModelUnsafe(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void InstallBingings()
        {
            //ObserveShow().SubscribeWithState(this, static (_, view) => view.viewModel.ForceNotify())
            //             .AddTo(this);
        }
    }
}
