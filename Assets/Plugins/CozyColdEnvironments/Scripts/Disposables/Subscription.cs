using CCEnvs.Returnables;
using System;
using System.Reflection;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Disposables
{
    public static class Subscription
    {
        public delegate void Disposer();
        public delegate void Disposer<in TObservable>(TObservable observable);
        public delegate void Disposer<in TObserver, in TObservable>(TObserver observer,
                                                                    TObservable observable);

        public static ISubscription Create(Disposer disposer)
        {
            return Create(observable: default(Unit), (_) => disposer());
        }
        public static ISubscription<TObservable> Create<TObservable>(
            TObservable observable,
            Disposer<TObservable> disposer)
        {
            return new Subscription<Unit, TObservable>((_, x) => disposer(x),
                                                       observer: default,
                                                       observable);
        }
        public static Subscription<TObserver, TObservable> Create<TObserver, TObservable>(
            TObserver observer,
            TObservable observable,
            Disposer<TObserver, TObservable> disposer)
        {
            return new Subscription<TObserver, TObservable>(disposer,
                                                            observer,
                                                            observable);
        }
    }

    public sealed class Subscription<TObserver, TObservable> 
        : ISubscription<TObserver, TObservable>
    {
        private readonly Subscription.Disposer<TObserver, TObservable> disposer;
        private readonly TObserver observer;
        private readonly TObservable observable;
        private bool disposedValue;

        public TObserver Observer => observer;
        public TObservable Observable => observable;

        public Subscription(Subscription.Disposer<TObserver, TObservable> disposer,
                            TObserver observer,
                            TObservable observable)
        {
            CC.Guard.IsNotNull(disposer, nameof(disposer));

            this.disposer = disposer;
            this.observer = observer;
            this.observable = observable;
        }

        /// <exception cref="InvalidOperationException"></exception>
        public void Dispose()
        {
            if (disposedValue)
                return;

            disposer(observer, observable);
            GC.SuppressFinalize(this);

            disposedValue = true;
        }
    }
}
