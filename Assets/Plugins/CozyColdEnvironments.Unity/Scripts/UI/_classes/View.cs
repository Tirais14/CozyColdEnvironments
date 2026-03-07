using System;
using System.Collections.Generic;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using R3;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    public abstract class View<TViewModel>
        : Showable,
        IView<TViewModel>

        where TViewModel : IViewModel
    {
        protected readonly List<IDisposable> viewModelDisposables = new();

        protected Lazy<TViewModel?> _viewModel = new(static () => default);

        public TViewModel? viewModel => _viewModel.Value;

        public object model => viewModelUnsafe.model;

        protected TViewModel viewModelUnsafe {
            get
            {
                if (viewModel.IsNull())
                    throw new InvalidOperationException("View model is not setted.");

                return viewModel;
            }
        }

        protected object modelUnsafe => viewModelUnsafe.model;

        protected override void Awake()
        {
            base.Awake();

            if (CreateViewModel().TryGetValue(out var vm))
                SetViewModel(vm);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TryDisposeViewModel();
        }

        /// <summary>
        /// Don't use previous <see cref="viewModel"/>, it has been disposed and don't use cached <see cref="viewModel"/> by the same reason.
        /// </summary>
        /// <param name="viewModel"></param>
        public void SetViewModel(TViewModel? viewModel)
        {
            TryDisposeViewModel();

            _viewModel = new Lazy<TViewModel?>(viewModel);

            if (_viewModel.Value.IsNotNull())
                Init();
        }

        /// <inheritdoc cref="SetViewModel(TViewModel?)"/>
        /// <param name="factory"></param>
        public void SetViewModelFactory(Func<TViewModel?> factory)
        {
            Guard.IsNotNull(factory);

            TryDisposeViewModel();

            _viewModel = new Lazy<TViewModel?>(() => factory());

            if (_viewModel.Value.IsNotNull())
                Init();
        }

        public T GetModel<T>()
        {
            return modelUnsafe.To<T>();
        }

        public T GetViewModel<T>()
            where T : IViewModel
        {
            return viewModelUnsafe.To<T>();
        }

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModel(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void Init()
        {
            if (viewModelUnsafe is not ViewModelBehaviour viewModelBehaviour)
                return;

            viewModelBehaviour.ObserveModel()
                .Where(this,
                static (model, @this) =>
                {
                    return @this.model != model;
                })
                .Subscribe(this,
                static (_, @this) =>
                {
                    var vm = @this.viewModelUnsafe;

                    @this.SetViewModel(default);
                    @this.SetViewModel(vm);
                })
                .AddTo(viewModelDisposables);
        }

        protected abstract Maybe<TViewModel> CreateViewModel();

        protected void TryDisposeViewModel()
        {
            if (_viewModel.IsValueCreated
                &&
                _viewModel.Value.Is<IDisposable>(out var disp))
            {
                viewModelDisposables.DisposeEachAndClear();

                disp.Dispose();
            }
        }
    }
}
