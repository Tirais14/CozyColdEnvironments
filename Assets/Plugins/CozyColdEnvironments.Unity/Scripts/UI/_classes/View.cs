using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using Cysharp.Threading.Tasks;
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

        private TViewModel? viewModel;

        private Func<TViewModel?>? viewModelFactory;

        private bool viewModelFactoryReturnsValue = true;
        private bool viewModelCreated;

        private IDisposable? modelBinding;

        public TViewModel? ViewModel => GetViewModel();

        public object? Model => ViewModel.Maybe().Map(static vm => vm.Model).GetValue();

        public bool SuppressViewModelCreation { get; set; }

        public Type ViewModelType => TypeofCache<TViewModel>.Type;

        protected TViewModel GuardedViewModel => ViewModel.ThrowIfNull(nameof(ViewModel));

        protected ICollection<IDisposable> ViewModelDisposables => viewModelDisposables.Value;

        protected override void Start()
        {
            base.Start();

            if (!SuppressViewModelCreation)
                _ = GetViewModel();
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

        public TViewModel? GetViewModel()
        {
            if (viewModel.IsNotNull())
                return viewModel;

            if (viewModelCreated
                ||
                !viewModelFactoryReturnsValue)
            {
                return default;
            }

            if (viewModelFactory == null)
                SetViewModelFactory(CreateViewModel);

            viewModel = viewModelFactory!();

            if (viewModel.IsNull())
            {
                viewModelFactoryReturnsValue = false;
                return default;
            }

            viewModelCreated = true;
            return viewModel;
        }

        /// <summary>
        /// Don't use previous <see cref="ViewModel"/>, it has been disposed and don't use cached <see cref="ViewModel"/> by the same reason.
        /// </summary>
        /// <param name="vm"></param>
        public void SetViewModel(TViewModel? vm)
        {
            TryDisposeViewModel();

            viewModelFactoryReturnsValue = true;

            OnSetViewModel(vm);

            viewModel = vm;

            if (vm.IsNotNull())
            {
                if (!didStart)
                    InitViewModelCoreAsync(vm).Forget();
                else
                    InitViewModelCore(vm);

                viewModelCreated = true;
            }
            else viewModelCreated = false;
        }

        /// <inheritdoc cref="SetViewModel(TViewModel?)"/>
        /// <param name="factory"></param>
        public void SetViewModelFactory(Func<TViewModel?>? factory)
        {
            TryDisposeViewModel();

            viewModelFactoryReturnsValue = true;
            viewModelCreated = false;

            OnSetViewModel(default);

            viewModelFactory = () =>
            {
                factory ??= CreateViewModel;

                var vm = factory();

                OnSetViewModel(vm);

                if (vm.IsNotNull())
                {
                    if (!didStart)
                        InitViewModelCoreAsync(vm).Forget();
                    else
                        InitViewModelCore(vm);
                }

                return vm;
            };
        }

        public bool HasModel() => Model.IsNotNull();
        public bool HasModel<T>() => Model.Is<T>();

        public bool HasViewModel() => ViewModel.IsNotNull();
        public bool HasViewModel<T>() => ViewModel.Is<T>();

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
            throw new NotImplementedException(nameof(InitViewModel));
        }

        protected virtual void OnSetViewModel(TViewModel? vm)
        {
            throw new NotImplementedException(nameof(OnSetViewModel));
        }

        protected abstract TViewModel? CreateViewModel();

        protected void TryDisposeViewModel()
        {
            if (viewModel is IDisposable disposable)
                disposable.Dispose();

            if (viewModelDisposables.IsValueCreated)
                ViewModelDisposables.DisposeEachAndClear(bufferized: true);

            modelBinding?.Dispose();
        }

        private void BindModel(TViewModel vm)
        {
            modelBinding = vm.ObserveModel()
                .Skip(1)
                .WhereNotNull()
                .Subscribe(OnModelChanged);
        }

        private void OnModelChanged(object? model)
        {
            CC.Guard.IsNotNull(ViewModel, nameof(ViewModel));

            SetViewModel(ViewModel);
        }

        private async UniTaskVoid InitViewModelCoreAsync(TViewModel vm)
        {
            await UniTask.WaitUntil(
                this,
                @this => @this.didStart,
                timing: PlayerLoopTiming.LastInitialization,
                cancellationToken: destroyCancellationToken
                );

            try
            {
                InitViewModel(vm);
                BindModel(vm);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void InitViewModelCore(TViewModel vm)
        {
            InitViewModel(vm);
            BindModel(vm);
        }
    }
}
