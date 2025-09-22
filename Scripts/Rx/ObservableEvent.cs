using CCEnvs.Disposables;
using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs
{
    [Obsolete]
    public struct ObservableEvent<T>
        : IObservable<T>,
        IEquatable<ObservableEvent<T>>
    {
        private Action<T>? action;

        public static bool operator ==(ObservableEvent<T> left, ObservableEvent<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObservableEvent<T> left, ObservableEvent<T> right)
        {
            return !(left == right);
        }

        public readonly bool Equals(ObservableEvent<T> other)
        {
            return action == other.action;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is ObservableEvent<T> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(action);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var observerAction = (Action<T>)observer.AsReflected()
                                                    .Method(nameof(IObserver<T>.OnNext)).value
                                                    .CreateDelegate(typeof(Action<T>));

            action += observerAction;

            return Subscription.Create(observer, this, (x, y) => y.action -= observerAction);
        }

        public readonly override string ToString() => action?.ToString() ?? "()";
    }
}
