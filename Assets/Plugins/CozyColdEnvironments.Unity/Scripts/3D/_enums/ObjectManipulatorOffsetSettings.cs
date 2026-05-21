using System;

namespace CCEnvs.UnityX.D3
{
    [Flags]
    public enum ObjectManipulatorOffsetSettings
    {
        None,
        ResetOnObjectChanged,
        Default = ResetOnObjectChanged
    }
}
