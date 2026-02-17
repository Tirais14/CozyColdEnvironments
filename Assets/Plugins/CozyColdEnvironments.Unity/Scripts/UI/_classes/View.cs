using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    public abstract class View<TViewModel>
        : GUITab,
        IView<TViewModel>

        where TViewModel : IViewModel
    {
        protected readonly List<IDisposable> viewModelDisposables = new();

        protected Lazy<TViewModel?> _viewModel = new(() => default);

        public Maybe<TViewModel> viewModel => _viewModel.Value;

        public Maybe<object> model => viewModel.Map(static x => x.model).GetValue();

        protected TViewModel viewModelUnsafe => viewModel.GetValueUnsafe(static () => throw new InvalidOperationException("View model is not setted."));
       
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
        public virtual void SetViewModel(TViewModel? viewModel)
        {
            TryDisposeViewModel();

            _viewModel = new Lazy<TViewModel?>(viewModel);

            if (_viewModel.Value.IsNotNull())
                Init();
        }

        /// <inheritdoc cref="SetViewModel(TViewModel?)"/>
        /// <param name="factory"></param>
        public virtual void SetViewModelFactory(Func<TViewModel?> factory)
        {
            Guard.IsNotNull(factory);

            TryDisposeViewModel();

            _viewModel = new Lazy<TViewModel?>(() => factory());

            if (_viewModel.Value.IsNotNull())
                Init();
        }

        //public virtual void SetModel(object model)
        //{
        //    CC.Guard.IsNotNull(model, nameof(model));
        //    CC.Guard.IsNotNull(viewModel.Raw, nameof(viewModel));

        //    viewModelUnsafe.Set
        //}

        public T GetModel<T>()
        {
            return modelUnsafe.To<T>();
        }

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModel(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void Init()
        {
        }

        protected abstract Maybe<TViewModel> CreateViewModel();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SelectableDoSelectPredicate() => viewModel.IsSome;

        protected void TryDisposeViewModel()
        {
            if (_viewModel.IsValueCreated
                &&
                _viewModel.Value.Is<IDisposable>(out var disp))
            {
                viewModelDisposables.DisposeEach();
                viewModelDisposables.Clear();

                disp.Dispose();
            }
        }
    }
}
