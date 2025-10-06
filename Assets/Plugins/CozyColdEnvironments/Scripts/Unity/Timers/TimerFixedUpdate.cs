using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public class TimerFixedUpdate : TimerMono
    {
        public override TimeSpan Interval => IsUnscaledInterval
            ?
            TimeSpan.FromSeconds(Time.fixedUnscaledDeltaTime)
            :
            TimeSpan.FromSeconds(Time.fixedDeltaTime);

        private void FixedUpdate() => Main();

        public static ITimer Create() => Create(UpdateType.FixedUpdate);
    }
}
