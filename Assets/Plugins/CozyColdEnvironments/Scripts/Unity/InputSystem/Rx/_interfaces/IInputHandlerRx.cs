using System;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public interface IInputHandlerRx : ISwitchable, IDisposable
    {
        InputActionMap ActionMap { get; }

        IInputActionRx GetInputAction(string inputName);
    }
}
