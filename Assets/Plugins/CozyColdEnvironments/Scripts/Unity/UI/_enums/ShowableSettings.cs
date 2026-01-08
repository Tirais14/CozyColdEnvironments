using System;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [Flags]
    public enum ShowableSettings
    {
        None,
        ShowHideByGameObjectState = 1,
        Default = ShowHideByGameObjectState
    }
}
