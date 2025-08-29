using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Timers
{
    public class TimerUpdate : TimerMono
    {
        private void Update()
        {
            if (!IsActive)
                return;

            timer.AddSeconds(Time.deltaTime);
        }

        public static ITimer Create()
        {
            return Create(UpdateType.Update);
        }
    }
}
