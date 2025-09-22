using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public class TimerFixedUpdate : TimerMono
    {
        protected override float DeltaTime => IsUnscaledInterval ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;

        private void FixedUpdate() => Main();

        public static ITimer Create() => Create(UpdateType.FixedUpdate);
    }
}
