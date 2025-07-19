using System;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem
{
    public interface IInputAction : IDisposable
    {
        bool IsButtonPressed { get; }

        event Action<CallbackContext> OnStarted;
        event Action<CallbackContext> OnPerformed;
        event Action<CallbackContext> OnCanceled;
    }
    public interface IInputAction<T> : IInputAction
        where T : struct
    {
        event Action<T> ValueOnStarted;
        event Action<T> ValueOnPerformed;
        event Action<T> ValueOnCanceled;

        T Value { get; }
    }
}
