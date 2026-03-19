using System;

namespace CCEnvs.Unity.D3
{
    [Flags]
    public enum ObjectManipulatorOffsetSettings
    {
        None,
        ResetOnObjectChanged,
        Default = ResetOnObjectChanged
    }
}
