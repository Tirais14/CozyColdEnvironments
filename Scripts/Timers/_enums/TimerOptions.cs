using System;

#nullable enable
namespace UTIRLib.Timers
{
    [Flags]
    public enum TimerOptions
    {
        None,
        ResetOnTargetReached,
        StopOnTargetReached = 2,
    }
}
