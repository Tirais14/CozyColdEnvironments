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
        CCBehaviour
    {
    }

    public abstract class ViewModelBehaviour<TModel> : ViewModelBehaviour,
        IViewModel<TModel>
    {
        private readonly ReactiveProperty<TModel?> model = new();

        private readonly Lazy<List<IDisposable>> modelDisposables = new(() => new List<IDisposable>());

        public TModel? Model => model.Value;    

        public virtual CancellationToken DisposeCancellationToken => destroyCancellationToken;

        protected ICollection<IDisposable> ModelDisposables => modelDisposables.Value;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetModel(default);
        }

        public virtual void SetModel(TModel? model)
        {
            if (modelDisposables.IsValueCreated)
                ModelDisposables.DisposeEachAndClear(bufferized: true);

            OnSetModel(model);

            this.model.Value = model;

            if (model.IsNotNull())
                InitModel(model);
        }

        public Observable<TModel?> ObserveModel() => model;

        protected virtual void OnSetModel(TModel? model)
        {
        }

        protected virtual void InitModel(TModel model)
        {

        }
    }
}
