using CCEnvs.Disposables;
using CCEnvs.Returnables;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Observables
{
    public class Observable<T> : IObservable<T>
    {
        private readonly Func<T> valueGetter;
        private readonly List<IObserver<T>> observers = new();

        public Observable(Func<T> valueGetter)
        {
            CC.Validate.ArgumentNull(valueGetter, nameof(valueGetter));

            this.valueGetter = valueGetter;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observers.Add(observer);
            var subscription = Subscription.Create(observer, this, (x, y) => y.observers.Remove(x));
            Publish();

            return subscription;
        }

        public void Publish()
        {
            foreach (var observer in observers)
            {
                bool error = false;
                try
                {
                    observer.OnNext(valueGetter());
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    error = true;
                }

                if (!error)
                    observer.OnCompleted();
            }
        }
    }
    public class Observable : Observable<Mock>
    {
        public Observable() : base(() => default)
        {
        }
    }
}
