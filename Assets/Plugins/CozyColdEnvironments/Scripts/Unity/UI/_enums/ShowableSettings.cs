using System;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [Flags]
    public enum ShowableSettings
    {
        None,
        HideByColor,
        KeepRaycastTargetState,
        Default = None
    }
}
