#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Threading;
using CCEnvs.TypeMatching;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#pragma warning disable S1699
namespace CCEnvs.UnityX.UI
{
    public abstract class ViewModel<TModel>
        :
        IViewModel<TModel>,
        IDisposable
    {
        private readonly ReactiveProperty<TModel?> model = new();

        private readonly Lazy<List<IDisposable>> modelDisposables = new(() => new List<IDisposable>());

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly CancellationTokenRegistration disposeCancellationTokenRegistration;

        public TModel? Model => model.Value;

        protected ICollection<IDisposable> ModelDisposables => modelDisposables.Value;

        protected TModel GuardedModel => Model.ThrowIfNull(nameof(Model));

        public CancellationToken DisposeCancellationToken {
            get => disposeCancellationTokenSource.Token;
        }

        protected ViewModel()
        {
            disposeCancellationTokenRegistration = DisposeCancellationToken.Register(
                static @this => @this.CastTo<ViewModel<TModel>>().Dispose(),
                this
                );
        }

        ~ViewModel() => Dispose();

        public virtual void SetModel(TModel? model)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

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
            return Model.As<T>().Let(out result).IsNotNull();
        }

        public Observable<TModel?> ObserveModel() => model;

        private int disposed;
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.CancelAndDispose();
                disposeCancellationTokenRegistration.Dispose();

                if (modelDisposables.IsValueCreated)
                    modelDisposables.Value.DisposeEachAndClear(bufferized: true);

                OnSetModel(default);
            }
        }

        /// <summary>
        /// Invokes on <see cref="SetModel(TModel?)"/> with value or in Dispose with always null
        /// </summary>
        /// <param name="model"></param>
        protected virtual void OnSetModel(TModel? model)
        {
            throw new NotImplementedException(string.Join('.', GetType().FullName, nameof(OnSetModel)));
        }

        protected virtual void InitModel(TModel model)
        {
            throw new NotImplementedException(string.Join('.', GetType().FullName, nameof(InitModel)));
        }
    }
}
