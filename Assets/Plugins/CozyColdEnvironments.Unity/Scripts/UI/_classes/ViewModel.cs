#nullable enable
using CCEnvs.Threading;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#pragma warning disable S1699
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModel<TModel> 
        :
        IViewModel<TModel>,
        IDisposable
    {
        private readonly ReactiveProperty<TModel?> model = new();

        private readonly Lazy<List<IDisposable>> modelDisposables = new(() => new List<IDisposable>());

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();
        private readonly CancellationTokenSource? linkedCancellationTokenSource;

        private readonly CancellationTokenRegistration disposeCancellationTokenRegistration;

        public TModel? Model => model.Value;

        protected ICollection<IDisposable> ModelDisposables => modelDisposables.Value;

        public CancellationToken DisposeCancellationToken {
            get => linkedCancellationTokenSource?.Token ?? disposeCancellationTokenSource.Token;
        }

        protected ViewModel(TModel? model, CancellationToken cancellationToken)
        {
            this.model.Value = model;

            linkedCancellationTokenSource = cancellationToken.LinkTokens(disposeCancellationTokenSource.Token);

            disposeCancellationTokenRegistration = DisposeCancellationToken.Register(
                static @this => @this.CastTo<ViewModel<TModel>>().Dispose(),
                this
                );
        }

        ~ViewModel() => Dispose();

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
                if (linkedCancellationTokenSource != null)
                {
                    linkedCancellationTokenSource.CancelAndDispose();
                    disposeCancellationTokenSource.Dispose();
                }
                else
                    disposeCancellationTokenSource.CancelAndDispose();

                disposeCancellationTokenRegistration.Dispose();

                if (modelDisposables.IsValueCreated)
                    modelDisposables.Value.DisposeEachAndClear(bufferized: true);
            }
        }

        protected virtual void OnSetModel(TModel? model)
        {
            throw new NotImplementedException(nameof(OnSetModel));
        }

        protected virtual void InitModel(TModel model)
        {
            throw new NotImplementedException(nameof(InitModel));
        }
    }
}
