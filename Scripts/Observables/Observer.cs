using CCEnvs.Returnables;
using System;

#nullable enable
namespace CCEnvs.Observables
{
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> onNext;
        private readonly Action<Exception>? onError;
        private readonly Action? onCompleted;

        public Observer(Action<T> onNext)
        {
            CC.Validate.ArgumentNull(onNext, nameof(onNext));

            this.onNext = onNext;
        }

        public Observer(Action<T> onNext, Action<Exception>? onError)
            :
            this(onNext)
        {
            this.onError = onError;
        }

        public Observer(Action<T> onNext, Action<Exception>? onError, Action? onCompleted)
            : 
            this(onNext, onError)
        {
            this.onCompleted = onCompleted;
        }

        public void OnNext(T value) => onNext(value);

        public void OnError(Exception error)
        {
            if (onError is null)
                throw error;

            onError(error);
        }

        public void OnCompleted() => onCompleted?.Invoke();
    }
    public class Observer : Observer<Mock>
    {
        public Observer(Action onNext) : base((_) => onNext())
        {
        }

        public Observer(Action onNext, Action<Exception>? onError)
            :
            base((_) => onNext(), onError)
        {
        }

        public Observer(Action onNext, Action<Exception>? onError, Action? onCompleted)
            :
            base((_) => onNext(), onError, onCompleted)
        {
        }
    }
}
