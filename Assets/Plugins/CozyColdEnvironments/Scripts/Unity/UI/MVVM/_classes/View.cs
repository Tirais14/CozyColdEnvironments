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

        public TModel model => viewModel.model;
        public TViewModel viewModel => _viewModel.Value;
        public virtual bool ViewModelMutable => false;

        protected override void Awake()
        {
            base.Awake();

            _viewModel = new Lazy<TViewModel>(() => ViewModelFactory().AddTo(this));
        }

        protected override void Start()
        {
            base.Start();
            SetupViewModel();
        }

        public virtual void SetViewModelUnsafe(TViewModel viewModel)
        {
            if (!ViewModelMutable)
                throw new InvalidOperationException("Cannot set new view model.");
            CC.Guard.IsNotNull(viewModel, nameof(viewModel));

            this.viewModel.Dispose();
            _viewModel = new Lazy<TViewModel>(viewModel);

            if (viewModel is IDisposable disp)
                disp.AddTo(this);

            SetupViewModel();
        }

        protected virtual TModel ModelFactory()
        {
            Type modelType = typeof(TModel);

            if (modelType.IsType<Component>())
                throw new InvalidOperationException($"Unity objects must be instantiated by Unity instead of creating new instance. Type: {modelType.GetFullName()}.");

            TModel model = typeof(TModel).Reflect().Cache().CreateInstance<TModel>();

            if (model is IDisposable disposable)
                disposable.AddTo(this);

            return model;
        }

        protected virtual TViewModel ViewModelFactory(TModel? model = default)
        {
            model = model.Maybe()
                         .IfNone(() => ModelFactory())
                         .AccessUnsafe()
                         .As<TModel>();

            return typeof(TViewModel).Reflect()
                                     .Cache()            
                                     .Arguments(model!, gameObject)
                                     .CreateInstance<TViewModel>();
        }

        /// <summary>
        /// Put any logic which needed to execute on <see cref="viewModel"/> changed.
        /// <br/>Called in <see cref="Start"/> and when <see cref="viewModel"/> is changed.
        /// </summary>
        protected abstract void SetupViewModel();
    }
}
