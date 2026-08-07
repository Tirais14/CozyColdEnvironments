using CCEnvs.Diagnostics;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.Components;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.UI
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

        protected TModel GuardedModel => Model.ThrowIfNull(nameof(Model));

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

        public bool HasModel() => Model.IsNotNull();
        public bool HasModel<T>() => Model.Is<T>();

        public bool TryGetModel([NotNullWhen(true)] out object? result)
        {
            result = Model;
            return Model.IsNotNull();
        }
        public bool TryGetModel<T>([NotNullWhen(true)] out T? result)
        {
            return Model.Is<T>(out result);
        }

        public Observable<TModel?> ObserveModel() => model;

        protected virtual void OnSetModel(TModel? model)
        {
            throw new NotImplementedException();
        }

        protected virtual void InitModel(TModel model)
        {
            throw new NotImplementedException();
        }
    }
}
