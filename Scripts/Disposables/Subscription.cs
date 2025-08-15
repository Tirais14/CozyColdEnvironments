using System;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Disposables
{
    public delegate void SubscriptionDisposer<TObserver, TObservable>(TObserver observer,
                                                                      TObservable observable);

    public static class Subscription
    {
        public static Subscription<TObserver, TObservable> 
            Create<TObserver, TObservable>(TObserver observer,
                                           TObservable observable, 
                                           SubscriptionDisposer<TObserver, TObservable> disposer)
        {
            return new Subscription<TObserver, TObservable>(disposer, observer, observable);
        }
    }

    public sealed class Subscription<TObserver, TObservable> 
        : 
        ISubscription<TObserver, TObservable>
    {
        private readonly SubscriptionDisposer<TObserver, TObservable> disposer;
        private readonly TObserver observer;
        private readonly TObservable observable;
        private bool disposedValue;

        public TObserver Observer => observer;
        public TObservable Observable => observable;

        public Subscription(SubscriptionDisposer<TObserver, TObservable> disposer,
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
