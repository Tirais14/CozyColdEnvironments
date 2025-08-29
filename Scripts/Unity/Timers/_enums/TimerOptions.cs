using System;

#nullable enable
namespace CozyColdEnvironments.Timers
{
    [Flags]
    public enum TimerOptions
    {
        None,
        ResetOnTargetReached,
        StopOnTargetReached = 2,
    }
}
