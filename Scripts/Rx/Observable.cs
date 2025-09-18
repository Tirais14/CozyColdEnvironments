using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Returnables;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Rx
{
    public class Observable<T> : IObservable<T>, IDisposable
    {
        protected readonly List<IObserver<T>> observers = new();

        private readonly T? value;
        private readonly Func<T>? valueGetter;
        private readonly bool useGetter;
        private readonly List<ISubscription> subscriptions;

        private volatile bool disposedValue;

        public Observable(T value)
        {
            this.value = value;
            useGetter = false;
        }

        public Observable(Func<T> valueGetter)
        {
            CC.Validate.ArgumentNull(valueGetter, nameof(valueGetter));

            this.valueGetter = valueGetter;
            useGetter = true;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            CC.Validate.ArgumentNull(observer, nameof(observer));

            observers.Add(observer);
            var subscription = Subscription.Create(observer, this, (x, _) => Unsubscribe(x));
            Publish();

            return subscription;
        }

        public void Unsubscribe(IObserver<T> observer)
        {
            CC.Validate.ArgumentNull(observer, nameof(observer));

            observers.Remove(observer);
        }

        public void Publish()
        {
            T tValue = useGetter ? valueGetter!() : value!;

            for (int i = 0; i < observers.Count;)
            {
                try
                {
                    observers[i].OnNext(tValue);
                    i++;
                }
                catch (Exception ex)
                {
                    CCDebug.PrintException(ex);
                    observers.RemoveAt(i);
                }
            }
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                subscriptions.ForEach(x => x.Dispose());
        }
    }
    public class Observable : Observable<Mock>, IObservable
    {
        public Observable() : base(value: default)
        {
        }

        public static Observable<T> Create<T>(T value)
        {
            return new Observable<T>(value);
        }
        public static Observable<T> Create<T>(Func<T> valueGetter)
        {
            return new Observable<T>(valueGetter);
        }

        //public static Observable FromEvent(Action<Action> addAction,
        //                                   Action<Action> removeAction)
        //{
        //    return 
        //}
    }
}
