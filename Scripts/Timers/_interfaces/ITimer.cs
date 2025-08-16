using System;

#nullable enable
namespace UTIRLib.Timers
{
    public interface ITimer
    {
        event Action<ITimer> OnTargetReached;

        float Seconds { get; }
        float TargetValue { get; }
        bool TargetValueReached { get; }
    }
}
