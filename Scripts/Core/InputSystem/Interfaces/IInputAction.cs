using System;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace UTIRLib.InputSystem
{
    public interface IInputAction : IDisposable
    {
        event Action<CallbackContext> OnPerformed;
    }
    public interface IInputAction<T> : IInputAction
        where T : struct
    {
        event Action<T> OnPerformedValue;

        T Value { get; }
    }
}
