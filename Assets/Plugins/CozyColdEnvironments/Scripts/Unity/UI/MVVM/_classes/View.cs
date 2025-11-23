using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
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
        : GUIPanel,
        IView<TViewModel>

        where TViewModel : IViewModel
    {
        protected Lazy<Maybe<TViewModel>> _viewModel = new(() => default);

        [Header("View Settings")]
        [Space(8)]

        [SerializeField]
        protected bool isMutableView;

        public Maybe<TViewModel> viewModel => _viewModel.Value;
        public Maybe<object> model => viewModel.Map(x => x.model).GetValue();
        public TViewModel viewModelUnsafe => viewModel.IfNone(() => throw new InvalidOperationException("View model is not setted.")).GetValueUnsafe();
        public object modelUnsafe => viewModelUnsafe.model;
        public bool IsMutable => isMutableView;

        protected override void Awake()
        {
            base.Awake();
            ObserveViewModel();
            ViewModelFactory().IfSome(viewModel => SetViewModelUnsafe(viewModel));
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

        public void SetViewModelUnsafe(TViewModel viewModel)
        {
            SetViewModelFactoryUnsafe(() => viewModel);
        }

        public void SetViewModelFactoryUnsafe(Func<TViewModel> factory)
        {
            Guard.IsNotNull(factory);

            if (_viewModel.Value.IsSome && !IsMutable)
                throw new InvalidOperationException("Cannot set view model.");

            _viewModel = new Lazy<Maybe<TViewModel>>(() => factory());
        }

        public Result<T> GetModel<T>()
        {
            try
            {
                return (modelUnsafe.As<T>(), null);
            }
            catch (Exception ex)
            {
                return (default, ex);
            }
        }

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModelUnsafe(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void Init()
        {
        }

        protected virtual Maybe<TViewModel> ViewModelFactory()
        {
            return Maybe<TViewModel>.None;
        }

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
