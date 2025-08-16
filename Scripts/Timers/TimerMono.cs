using UnityEngine;

#nullable enable
namespace UTIRLib.Timers
{
    public class TimerMono : ATimerMono
    {
        private void Update()
        {
            if (!IsEnabled)
                return;

            timer.AddSeconds(Time.deltaTime);
        }
    }
}
