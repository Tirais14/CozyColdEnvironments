using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.InputSystem.Reactive
{
    public interface IInputHandlerReactive : ISwitchable, IDisposable
    {
        InputActionMap ActionMap { get; }

        IInputActionReactive GetInputAction(string inputName);
    }
}
