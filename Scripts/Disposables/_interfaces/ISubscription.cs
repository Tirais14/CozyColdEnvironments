using System;

#nullable enable
namespace CozyColdEnvironments.Disposables
{
    public interface ISubscription : IDisposable
    {
    }

    public interface ISubscription<out TObservable> : ISubscription
    {
        TObservable Observable { get; }
    }

    public interface ISubscription<out TObserver, out TObservable> : ISubscription<TObservable>
    {
        TObserver Observer { get; }
    }
}
