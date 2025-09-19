using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.Returnables;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Rx
{
    public class Observable<T> : IObservable<T>, IDisposable
    {
        protected readonly List<IObserver<T>> observers = new();

        private readonly T? target;
        private readonly object? targetRef;
        private readonly TargetRefType targetRefType;
        private readonly List<ISubscription> subscriptions = new();

        private volatile bool disposedValue;

        public Observable(T target)
        {
            this.target = target;
            targetRefType = TargetRefType.Value;
        }

        public Observable(Func<T> targetGetter)
        {
            CC.Validate.ArgumentNull(targetGetter, nameof(targetGetter));

            targetRef = targetGetter;
            targetRefType = TargetRefType.Getter;
        }

        public Observable(ContextedFieldInfo targetContainer)
        {
            CC.Validate.ArgumentNull(targetContainer, nameof(targetContainer));

            targetRef = targetContainer;
            targetRefType = TargetRefType.Field;
        }

        public Observable(ContextedPropertyInfo targetContainer)
        {
            CC.Validate.ArgumentNull(targetContainer, nameof(targetContainer));

            targetRef = targetContainer;
            targetRefType = TargetRefType.Property;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            CC.Validate.ArgumentNull(observer, nameof(observer));

            observers.Add(observer);
            var subscription = Subscription.Create(observer, this, (x, _) => Unsubscribe(x));
            subscriptions.Add(subscription);
            Publish();

            return subscription;
        }

        public void Unsubscribe(IObserver<T> observer)
        {
            CC.Validate.ArgumentNull(observer, nameof(observer));

            observers.Remove(observer);
        }

#pragma warning disable S127
        public void Publish()
        {
            T tTarget;

            if (targetRefType == TargetRefType.Value)
                tTarget = target!;
            else
            {
                tTarget = targetRefType switch
                {
                    TargetRefType.Getter => targetRef.As<Func<T>>().Invoke(),
                    TargetRefType.Field => targetRef.As<ContextedFieldInfo>().GetValue().As<T>(),
                    TargetRefType.Property => targetRef.As<ContextedPropertyInfo>().GetValue().As<T>(),
                    _ => throw new InvalidOperationException(targetRefType.ToString())
                };
            }

            for (int i = 0; i < observers.Count;)
            {
                try
                {
                    observers[i].OnNext(tTarget);
                    i++;
                }
                catch (Exception ex)
                {
                    CCDebug.PrintException(ex);
                    observers.RemoveAt(i);
                }
            }
        }
#pragma warning restore S127

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                subscriptions.ForEach(x => x.Dispose());

            disposedValue = true;
        }

        private enum TargetRefType
        {
            None,
            Value,
            Getter,
            Field,
            Property
        }
    }
    public class Observable : Observable<Mock>, IObservable
    {
        public Observable() : base(target: default)
        {
        }

        public static Observable<T> Create<T>(T target)
        {
            return new Observable<T>(target);
        }
        public static Observable<T> Create<T>(Func<T> targetGetter)
        {
            return new Observable<T>(targetGetter);
        }
        public static Observable<T> Create<T>(ContextedFieldInfo targetContainer)
        {
            return new Observable<T>(targetContainer);
        }
        public static Observable<T> Create<T>(ContextedPropertyInfo targetContainer)
        {
            return new Observable<T>(targetContainer);
        }
    }
}
