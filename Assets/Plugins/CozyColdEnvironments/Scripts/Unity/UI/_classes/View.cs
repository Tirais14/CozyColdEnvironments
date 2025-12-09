using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;

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
            DisposeViewModel(_viewModel);
        }

        protected static void DisposeViewModel(Lazy<Maybe<TViewModel>> viewModel)
        {
            if (viewModel.IsValueCreated
                &&
                viewModel.Value.TryGetValue(out TViewModel? value)
                &&
                value is IDisposable disp)
            {
                disp.Dispose();
            }
        }

        public void SetViewModel(TViewModel? viewModel)
        {
            _viewModel = new Lazy<Maybe<TViewModel>>(viewModel.Maybe()); 
        }

        public void SetViewModelFactory(Func<TViewModel> factory)
        {
            Guard.IsNotNull(factory);

            _viewModel = new Lazy<Maybe<TViewModel>>(() => factory());
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

        private void ObserveViewModel()
        {
            this.ObserveEveryValueChanged(static (@this) => @this._viewModel)
                .Pairwise()
                .SubscribeWithState(this, (pair, @this) =>
                {
                    DisposeViewModel(pair.Previous);

                    if (pair.Current.Value.IsSome)
                    {
                        if (pair.Current.Value.Raw is IDisposable disp)
                            disp.AddTo(this);

                        Init();
                    }
                })
                .AddTo(this);
        }
    }
}
