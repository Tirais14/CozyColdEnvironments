using System;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    [Flags]
    public enum TimerOptions
    {
        None,
        ResetOnTargetReached,
        StopOnTargetReached = 2,
    }
}
