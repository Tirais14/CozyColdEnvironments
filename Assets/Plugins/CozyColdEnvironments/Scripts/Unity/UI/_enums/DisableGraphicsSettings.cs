using System;

namespace CCEnvs.Unity.UI
{
    [Flags]
    public enum DisableGraphicsSettings
    {
        None = 0,
        Default = None,
        KeepRaycastTargetState = 1
    }
}
