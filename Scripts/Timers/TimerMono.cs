using UnityEngine;

#nullable enable
namespace UTIRLib.Timers
{
    public class TimerMono : ATimerMono
    {
        private void Update()
        {
            timer.AddSeconds(Time.deltaTime);
        }
    }
}
