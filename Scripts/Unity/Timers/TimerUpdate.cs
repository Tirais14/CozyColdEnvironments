using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public class TimerUpdate : TimerMono
    {
        protected override float DeltaTime => Time.deltaTime;

        public static TimerUpdate Create() => (TimerUpdate)Create(UpdateType.Update);

        private void Update() => Main();
    }
}
