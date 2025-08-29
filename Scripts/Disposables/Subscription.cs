using System;

#nullable enable
#pragma warning disable S3881
namespace CozyColdEnvironments.Disposables
{
    public static class Subscription
    {
        public delegate void Disposer<TObservable>(TObservable observable);
        public delegate void Disposer<TObserver, TObservable>(TObserver observer,
                                                              TObservable observable);

        public static Subscription<TObserver, TObservable> 
            Create<TObserver, TObservable>(TObserver observer,
                                           TObservable observable, 
                                           Disposer<TObserver, TObservable> disposer)
        {
            return new Subscription<TObserver, TObservable>(disposer, observer, observable);
        }
    }

    public sealed class Subscription<TObservable>
    :
    ISubscription<TObservable>
    {
        private readonly Subscription.Disposer<TObservable> disposer;
        private readonly TObservable observable;
        private bool disposedValue;

        public TObservable Observable => observable;

        public Subscription(Subscription.Disposer<TObservable> disposer,
                            TObservable observable)
        {
            this.disposer = disposer;
            this.observable = observable;
        }

        /// <exception cref="InvalidOperationException"></exception>
        public void Dispose()
        {
            if (disposedValue)
                return;

            disposer(observable);
            GC.SuppressFinalize(this);

            disposedValue = true;
        }
    }

    public sealed class Subscription<TObserver, TObservable> 
        : 
        ISubscription<TObserver, TObservable>
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
