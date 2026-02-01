#nullable enable
using CCEnvs.Reflection;
using R3;
using System;
using System.Linq;
using System.Threading;

#pragma warning disable S1699
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModel<TModel> : IViewModel<TModel>, IDisposable
    {
        protected readonly CompositeDisposable disposables = new();
        protected readonly CancellationToken cancellationToken;

        private readonly CancellationTokenRegistration cancellationTokenRegistration;

        private bool disposed;
        private bool isDisposing;

        public TModel model { get; private set; }

        protected ViewModel(TModel model, CancellationToken cancellationToken = default)
        {
            this.cancellationToken = cancellationToken;
            this.model = model;

            cancellationTokenRegistration = cancellationToken.Register(
                static @this =>
                {
                    var typed = @this.To<ViewModel<TModel>>();

                    if (typed.isDisposing)
                        return;

                    typed.Dispose();
                },
                this
                );

            AddDisposableViewModelDataToList();
        }

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            isDisposing = true;

            if (disposing)
            {
                cancellationTokenRegistration.Dispose();

                disposables.DisposeEach();
                disposables.Clear();
            }

            isDisposing = false;
            disposed = true;
        }

        protected virtual void AddDisposableViewModelDataToList()
        {
            foreach (var item in this.Reflect()
                .Cache()
                .Fields()
                .Where(field =>
                {
                    return field.Name != nameof(disposables)
                           &&
                           field.Name != nameof(cancellationTokenRegistration);
                })
                .Select(field => field.GetValue(this))
                .OfType<IDisposable>())
            {
                disposables.Add(item);
            }
        }
    }
}
