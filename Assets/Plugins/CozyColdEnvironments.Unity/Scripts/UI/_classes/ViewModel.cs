#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CCEnvs.Reflection;
using CCEnvs.Threading;
using R3;

#pragma warning disable S1699
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModel<TModel> : IViewModel<TModel>, IDisposable
    {
        protected readonly List<IDisposable> disposables = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        public TModel model { get; }

        public CancellationToken DisposeCancellationToken { get; }

        protected ViewModel(TModel model, CancellationToken cancellationToken)
        {
            this.model = model;

            var linkedTokenSource = cancellationToken.LinkTokens(disposeCancellationTokenSource.Token);

            linkedTokenSource.AddTo(disposables);

            DisposeCancellationToken = linkedTokenSource.Token;

            DisposeCancellationToken.Register(
                static @this =>
                {
                    var typed = @this.To<ViewModel<TModel>>();

                    typed.Dispose();
                },
                this
                )
                .AddTo(disposables);

            AddDisposableViewModelDataToList();
        }

        private bool disposed;
        public void Dispose() => Dispose(disposing: true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.Cancel();
                disposeCancellationTokenSource.Dispose();

                disposables.DisposeEach();
                disposables.Clear();
            }

            disposed = true;
        }

        [Obsolete]
        protected virtual void AddDisposableViewModelDataToList()
        {
            foreach (var item in this.Reflect()
                .Cache()
                .Fields()
                .Where(field =>
                {
                    return field.Name != nameof(disposables);
                })
                .Select(field => field.GetValue(this))
                .OfType<IDisposable>())
            {
                disposables.Add(item);
            }
        }
    }
}
