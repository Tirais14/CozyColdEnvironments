using CCEnvs.Returnables;
using System;

#nullable enable
namespace CCEnvs.Rx
{
    public static class IObservableExtensions
    {
        //public static IDisposable Subscribe<T>(this IObservable<T> value,
        //                                       Action<T> onNext,
        //                                       Action<Exception>? onError = null,
        //                                       Action? onCompleted = null)
        //{
        //    CC.Validate.ArgumentNull(value, nameof(value));
        //    CC.Validate.ArgumentNull(onNext, nameof(onNext));

        //    return value.Subscribe(new Observer<T>(onNext, onError, onCompleted));
        //}
        //public static IDisposable Subscribe(this IObservable<Mock> value,
        //                                    Action onNext,
        //                                    Action<Exception>? onError = null,
        //                                    Action? onCompleted = null)
        //{
        //    CC.Validate.ArgumentNull(value, nameof(value));
        //    CC.Validate.ArgumentNull(onNext, nameof(onNext));

        //    return value.Subscribe(new Observer(onNext, onError, onCompleted));
        //}
    }
}
