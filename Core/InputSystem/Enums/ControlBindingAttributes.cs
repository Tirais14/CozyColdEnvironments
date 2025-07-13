using System;

#nullable enable

namespace UTIRLib.InputSystem
{
    [Flags]
    public enum ControlBindingAttributes
    {
        OnPerformed = 2,
        OnStarted = 4,
        OnCancelled = 8,
        Button = 16,
        Vector2 = 32,
        Vector3 = 64,
        Quaternion = 128
    }
}