using CCEnvs.Unity.Components;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModelBehaviour
        :
        CCBehaviour,
        IViewModel,
        IDisposable
    {
        protected readonly ReactiveProperty<object> _model = new();

        protected readonly List<IDisposable> modelDisposables = new();

        public object model => _model.Value;

        public virtual CancellationToken DisposeCancellationToken => destroyCancellationToken;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _model.Dispose();
        }

        public ViewModelBehaviour SetModelUntyped(object model)
        {
            CC.Guard.IsNotNull(model, nameof(model));

            modelDisposables.DisposeEachAndClear();

            _model.Value = model;

            Init();

            return this;
        }

        public Observable<object> ObserveModel()
        {
            return _model;
        }

        protected virtual void Init()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }

    public abstract class ViewModelBehaviour<TModel> : ViewModelBehaviour,
        IViewModel<TModel>
    {
        new public TModel model => (TModel)_model.Value;

        public ViewModelBehaviour<TModel> SetModelUntyped(TModel model)
        {
            return (ViewModelBehaviour<TModel>)SetModelUntyped((object)model!);
        }

        new public Observable<TModel> ObserveModel()
        {
            return _model.Cast<object, TModel>();
        }
    }

    public static class ViewModelBehaviourExtensions
    {
        public static TViewModel SetModel<TViewModel, TModel>(
            this TViewModel source,
            TModel model
            )
            where TViewModel : ViewModelBehaviour<TModel>
        {
            CC.Guard.IsNotNullSource(source);

            return (TViewModel)source.SetModelUntyped(model);
        }

        public static TViewModel SetModel<TViewModel>(
            this TViewModel source,
            object model
            )
            where TViewModel : ViewModelBehaviour
        {
            CC.Guard.IsNotNullSource(source);

            return (TViewModel)source.SetModelUntyped(model);
        }
    }
}
