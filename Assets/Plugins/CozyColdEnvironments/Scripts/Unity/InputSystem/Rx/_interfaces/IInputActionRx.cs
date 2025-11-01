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

        bool IsButtonPressed();
    }
    public interface IInputActionRx<out T> : IInputActionRx
        where T : struct
    {
        T InputValue { get; }

        IObservable<T> TRaw { get; }
        IObservable<T> TStarted { get; }
        IObservable<T> TPerformed { get; }
        IObservable<T> TCanceled { get; }
    }
}
