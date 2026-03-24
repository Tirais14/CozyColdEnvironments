using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    public abstract class View : Showable
    {

    }
    public abstract class View<TViewModel>
        : 
        View,
        IView<TViewModel>

        where TViewModel : IViewModel
    {
        private readonly Lazy<List<IDisposable>> viewModelDisposables = new(() => new List<IDisposable>());

        private Lazy<TViewModel?> viewModel = new(static () => default);

        private IDisposable? modelBinding;

        public TViewModel? ViewModel => viewModel.Value;

        public object? Model => ViewModel.Maybe().Map(static vm => vm.Model);

        public bool SuppressViewModelCreation { get; set; }

        protected TViewModel GuardedViewModel => ViewModel.ThrowIfNull(nameof(ViewModel));

        protected ICollection<IDisposable> ViewModelDisposables => viewModelDisposables.Value;

        protected override void Start()
        {
            base.Start();

            if (!SuppressViewModelCreation && CreateViewModel().TryGetValue(out var vm))
                SetViewModel(vm);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TryDisposeViewModel();
            OnSetViewModel(default);
        }

        public View<TViewModel> SetSuppressViewModelCreation(bool value)
        {
            SuppressViewModelCreation = value;
            return this;
        }

        /// <summary>
        /// Don't use previous <see cref="ViewModel"/>, it has been disposed and don't use cached <see cref="ViewModel"/> by the same reason.
        /// </summary>
        /// <param name="viewModel"></param>
        public void SetViewModel(TViewModel? viewModel)
        {
            TryDisposeViewModel();

            OnSetViewModel(viewModel);

            this.viewModel = new Lazy<TViewModel?>(viewModel);

            if (viewModel.IsNotNull())
                InitViewModel(viewModel);
        }

        /// <inheritdoc cref="SetViewModel(TViewModel?)"/>
        /// <param name="factory"></param>
        public void SetViewModelFactory(Func<TViewModel?> factory)
        {
            Guard.IsNotNull(factory);

            TryDisposeViewModel();

            viewModel = new Lazy<TViewModel?>(() =>
            {
                var vm = factory();

                if (vm.IsNotNull())
                    InitViewModel(vm);

                return vm;
            });
        }

        public T GetModel<T>()
        {
            return Model.CastTo<T>();
        }

        public T GetViewModel<T>()
            where T : IViewModel
        {
            return ViewModel.CastTo<T>();
        }

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModel(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void InitViewModel(TViewModel vm)
        {
            BindModel(vm);
        }

        protected virtual void OnSetViewModel(TViewModel? vm)
        {
            throw new NotImplementedException(nameof(OnSetViewModel));
        }

        protected abstract Maybe<TViewModel> CreateViewModel();

        protected void TryDisposeViewModel()
        {
            if (viewModel.IsValueCreated
                &&
                ViewModel is IDisposable disposable)
            {
                disposable?.Dispose();
            }

            if (viewModelDisposables.IsValueCreated)
                ViewModelDisposables.DisposeEachAndClear(bufferized: true);

            modelBinding?.Dispose();
        }

        private void BindModel(TViewModel vm)
        {
            modelBinding = vm.ObserveModel()
                .Subscribe(OnModelChanged);
        }

        private void OnModelChanged(object? model)
        {
            CC.Guard.IsNotNull(ViewModel, nameof(ViewModel));

            InitViewModel(ViewModel);
        }
    }
}
