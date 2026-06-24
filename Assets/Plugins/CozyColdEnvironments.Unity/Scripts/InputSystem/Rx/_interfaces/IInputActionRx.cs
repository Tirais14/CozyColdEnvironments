using R3;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.UnityX.InputSystem.Rx
{
    public interface IInputActionRx
        :
        IDisposable,
        ISwitchable
    {
        InputAction Action { get; }

        string Name { get; }

        bool WasStartedOnThisFrame { get; }
        bool WasPerformedOnThisFrame { get; }
        bool WasCanceledOnThisFrame { get; }

        bool IsButtonPressed();

        T ReadValue<T>() where T : struct;

        Observable<CallbackContext> ObserveRaw();

        Observable<CallbackContext> ObserveStarted();

        Observable<CallbackContext> ObservePerformed();

        //Observable<CallbackContext> ObservePerformedContinuous();

        Observable<CallbackContext> ObserveCanceled();
    }
    public interface IInputActionRx<T> : IInputActionRx
        where T : struct
    {
        T OnStartedValue { get; }
        T OnPerformedValue { get; }
        T OnCanceledValue { get; }

        T ReadValue();

        Observable<T> ObserveRawValue();

        Observable<T> ObserveStartedValue();

        Observable<T> ObservePerformedValue();

        //Observable<T> ObservePerformedValueContinuous();

        Observable<T> ObserveCanceledValue();
    }
}
