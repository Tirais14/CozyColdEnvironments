using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Timers
{
    public class TimerLateUpdate : TimerMono
    {
        private void LateUpdate()
        {
            if (!IsActive)
                return;

            timer.AddSeconds(Time.deltaTime);
        }

        public static ITimer Create()
        {
            return Create(UpdateType.LateUpdate);
        }
    }
}
