using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using R3;
using UnityEngine;
using CCEnvs.Unity.Components;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    /// <summary>
    /// <see cref="TModel"/> must have empty constructor by default
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class View<TViewModel>
        : GUITab,
        IView<TViewModel>

        where TViewModel : IViewModel
    {
        protected Lazy<Maybe<TViewModel>> _viewModel = new(() => default);

        [Header("View Settings")]
        [Space(8)]

        [SerializeField]
        protected bool isMutable;

        public Maybe<TViewModel> viewModel => _viewModel.Value;
        public Maybe<object> model => viewModel.Map(x => x.model).GetValue();
        protected TViewModel viewModelUnsafe => viewModel.IfNone(() => throw new InvalidOperationException("View model is not setted.")).GetValueUnsafe();
        protected object modelUnsafe => viewModelUnsafe.model;

        protected override void Awake()
        {
            base.Awake();
            ObserveViewModel();
            ViewModelFactory().IfSome(viewModel => SetViewModel(viewModel));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TryDisposeViewModel(_viewModel);
        }

        protected static void TryDisposeViewModel(Lazy<Maybe<TViewModel>> viewModel)
        {
            if (viewModel.IsValueCreated && viewModel.Value.TryGetValue(out var vm)
                &&
                vm.Is<IDisposable>(out var disp))
            {
                disp.Dispose();
            }
        }

        /// <summary>
        /// Don't use previous <see cref="viewModel"/>, it has been disposed and don't use cached <see cref="viewModel"/> by the same reason.
        /// </summary>
        /// <param name="viewModel"></param>
        public void SetViewModel(TViewModel? viewModel)
        {
            OnViewModelChanging();
            TryDisposeViewModel(_viewModel);
            _viewModel = new Lazy<Maybe<TViewModel>>(viewModel.Maybe());
            OnViewModelChanged();
        }

        /// <inheritdoc cref="SetViewModel(TViewModel?)"/>
        /// <param name="factory"></param>
        public void SetViewModelFactory(Func<TViewModel> factory)
        {
            Guard.IsNotNull(factory);

            OnViewModelChanging();
            TryDisposeViewModel(_viewModel);
            _viewModel = new Lazy<Maybe<TViewModel>>(() => factory());
            OnViewModelChanged();
        }

        public Result<T> GetModel<T>()
        {
            try
            {
                return (modelUnsafe.To<T>(), null);
            }
            catch (Exception ex)
            {
                return (default, ex);
            }
        }

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModel(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void Init()
        {
        }

        protected virtual Maybe<TViewModel> ViewModelFactory()
        {
            return Maybe<TViewModel>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SelectableDoSelectPredicate() => viewModel.IsSome;

        protected virtual void OnViewModelChanging()
        {

        }

        protected virtual void OnViewModelChanged()
        {
        }

        private void ObserveViewModel()
        {
            Observable.EveryValueChanged(this,
                static (@this) => @this._viewModel)
                .Pairwise()
                .Subscribe(this, static (pair, @this) =>
                {
                    TryDisposeViewModel(pair.Previous);

                    if (pair.Current.Value.IsSome)
                    {
                        if (pair.Current.Value.Raw is IDisposable disp)
                            disp.BindTo(@this);

                        @this.Init();
                    }
                })
                .BindTo(this);
        }
    }
}
