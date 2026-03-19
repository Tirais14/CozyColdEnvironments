using System;

namespace CCEnvs.Unity.D3
{
    [Flags]
    public enum ObjectManipulatorVelocitySettings
    {
        None,
        ResetOnSet,
        ResetOnDrop,
        Default = ResetOnSet | ResetOnDrop
    }
}
