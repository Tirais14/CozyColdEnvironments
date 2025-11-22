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

        public void SetViewModelUnsafe(TViewModel viewModel)
        {
            if (_viewModel.Value.IsSome && !IsMutable)
                throw new InvalidOperationException("Cannot set view model.");

            _viewModel.AsOrDefault<IDisposable>().IfSome(static viewModel => viewModel.Dispose());
            _viewModel = new Lazy<Maybe<TViewModel>>(viewModel);

            if (viewModel is IDisposable disp)
                disp.AddTo(this);

            Init();
        }

        public void SetViewModelFactoryUnsafe(Func<TViewModel> factory)
        {
            Guard.IsNotNull(factory);

            _viewModel = new Lazy<Maybe<TViewModel>>(() =>
            {
                var viewModel = factory();
                SetViewModelUnsafe(viewModel);

                return viewModel;
            });
        }

        public T GetModelUnsafe<T>()
        {
            return modelUnsafe.As<T>();
        }

        public Maybe<T> GetModel<T>() => model.Raw.AsOrDefault<T>();

        /// <summary>
        /// Invokes in <see cref="Start"/>, <see cref="SetViewModelUnsafe(TViewModel)"/>, <see cref="SetModelUnsafe(TModel)"/>
        /// </summary>
        protected virtual void Init()
        {
        }
    }
}
