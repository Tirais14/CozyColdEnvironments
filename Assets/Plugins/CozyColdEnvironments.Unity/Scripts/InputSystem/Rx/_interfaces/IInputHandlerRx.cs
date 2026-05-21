using System;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.UnityX.InputSystem.Rx
{
    public interface IInputHandlerRx : IDisposable, ISwitchable
    {
        InputActionMap ActionMap { get; }

        IInputActionRx GetInputAction(string inputName);
    }
}
