using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Reactive
{
    public interface IInputActionReactive
        :
        ISwitchable,
        IDisposable
    {
        InputAction Action { get; }
        string ActionName { get; }
        IObservable<CallbackContext> Raw { get; }
        IObservable<CallbackContext> Started { get; }
        IObservable<CallbackContext> Performed { get; }
        IObservable<CallbackContext> Canceled { get; }
        IObservable<bool> ButtonRaw { get; }
        IObservable<bool> ButtonStarted { get; }
        IObservable<bool> ButtonPerformed { get; }
        IObservable<bool> ButtonCanceled { get; }

        bool IsButtonPressed();
    }
    public interface IInputActionReactive<T> : IInputActionReactive
        where T : struct
    {
        T Value { get; }

        IObservable<T> TRaw { get; }
        IObservable<T> TStarted { get; }
        IObservable<T> TPerformed { get; }
        IObservable<T> TCanceled { get; }
    }
}
