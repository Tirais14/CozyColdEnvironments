using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public class TimerUpdate : TimerMono
    {
        public override TimeSpan Interval => IsUnscaledInterval
                        ?
                        TimeSpan.FromSeconds(Time.unscaledDeltaTime)
                        :
                        TimeSpan.FromSeconds(Time.deltaTime);

        public static TimerUpdate Create() => (TimerUpdate)Create(UpdateType.Update);

        private void Update() => Main();
    }
}
