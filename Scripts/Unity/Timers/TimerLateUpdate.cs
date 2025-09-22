using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public class TimerLateUpdate : TimerMono
    {
        public override TimeSpan Interval => IsUnscaledInterval
            ?
            TimeSpan.FromSeconds(Time.unscaledDeltaTime)
            : 
            TimeSpan.FromSeconds(Time.deltaTime);

        private void LateUpdate() => Main();

        public static ITimer Create() => Create(UpdateType.LateUpdate);
    }
}
