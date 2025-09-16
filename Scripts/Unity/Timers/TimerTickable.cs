using CCEnvs.Unity.Tickables;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    [Obsolete("In developing")]
    public class TimerTickable : ATimerTickable, ITickable
    {
        bool ITickableBase.IsTickableEnabled {
            get => IsActive;
            set
            {
                if (value)
                    StartTimer();
                else
                    StopTimer();
            }
        }
    }
}
