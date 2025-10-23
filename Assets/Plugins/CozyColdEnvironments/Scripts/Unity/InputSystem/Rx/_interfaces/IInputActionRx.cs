using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public interface IInputActionRx
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
    public interface IInputActionRx<T> : IInputActionRx
        where T : struct
    {
        T InputValue { get; }

        IObservable<T> TRaw { get; }
        IObservable<T> TStarted { get; }
        IObservable<T> TPerformed { get; }
        IObservable<T> TCanceled { get; }
    }
}
