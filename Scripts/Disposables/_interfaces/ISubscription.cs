using System;
using UnityEngine;

#nullable enable
namespace UTIRLib.Disposables
{
    public interface ISubscription : IDisposable
    {
    }

    public interface ISubscription<out TObserver, out TObservable> : ISubscription
    {
        TObserver Observer { get; }
        TObservable Observable { get; }
    }
}
