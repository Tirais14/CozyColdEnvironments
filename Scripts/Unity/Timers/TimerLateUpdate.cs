using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public class TimerLateUpdate : TimerMono
    {
        protected override float DeltaTime => Time.deltaTime;

        private void LateUpdate() => Main();

        public static ITimer Create() => Create(UpdateType.LateUpdate);
    }
}
