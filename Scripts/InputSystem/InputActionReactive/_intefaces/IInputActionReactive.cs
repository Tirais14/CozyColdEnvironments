using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
{
    public interface IInputActionReactive
        :
        ISwitchable,
        IDisposable
    {
        InputAction Action { get; }

        IObservable<CallbackContext> Started { get; }
        IObservable<CallbackContext> Performed { get; }
        IObservable<CallbackContext> Canceled { get; }

        bool IsButtonPressed();
    }
    public interface IInputActionReactive<T> : IInputActionReactive
        where T : struct
    {
        T Value { get; }

        IObservable<T> StartedT { get; }
        IObservable<T> PerformedT { get; }
        IObservable<T> CanceledT { get; }
    }
}
