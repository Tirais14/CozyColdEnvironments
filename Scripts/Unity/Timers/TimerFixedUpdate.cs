using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Timers
{
    public class TimerFixedUpdate : TimerMono
    {
        private void FixedUpdate()
        {
            if (!IsActive)
                return;

            timer.AddSeconds(Time.fixedDeltaTime);
        }

        public static ITimer Create()
        {
            return Create(UpdateType.FixedUpdate);
        }
    }
}
