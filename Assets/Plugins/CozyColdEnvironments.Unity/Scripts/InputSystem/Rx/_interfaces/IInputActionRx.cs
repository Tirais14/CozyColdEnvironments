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

        bool IsButtonPressed();

        Observable<CallbackContext> ObserveRaw();

        Observable<CallbackContext> ObserveStarted();

        Observable<CallbackContext> ObservePerformed();

        Observable<CallbackContext> ObserveCanceled();
    }
    public interface IInputActionRx<T> : IInputActionRx
        where T : struct
    {
        T InputValue { get; }

        Observable<T> ObserveRawValue();

        Observable<T> ObserveStartedValue();

        Observable<T> ObservePerformedValue();

        Observable<T> ObserveCanceledValue();
    }
}
