using System;
using R3;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public interface IInputActionRx
        :
        IDisposable,
        ISwitchable
    {
        InputAction Action { get; }
        string ActionName { get; }

        Observable<CallbackContext> Raw { get; }
        Observable<CallbackContext> Started { get; }
        Observable<CallbackContext> Performed { get; }
        Observable<CallbackContext> Canceled { get; }

        bool IsButtonPressed();
    }
    public interface IInputActionRx<T> : IInputActionRx
        where T : struct
    {
        T InputValue { get; }

        Observable<T> TRaw { get; }
        Observable<T> TStarted { get; }
        Observable<T> TPerformed { get; }
        Observable<T> TCanceled { get; }
    }
}
