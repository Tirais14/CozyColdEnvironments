using System;

namespace CCEnvs.Unity.UI
{
    [Flags]
    public enum ShowableSettings
    {
        None = 0,
        KeepRaycastTargetState = 1,
        Recursive = 2,
        Default = None,
    }
}
