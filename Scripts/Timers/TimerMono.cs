using UnityEngine;

#nullable enable
namespace UTIRLib.Timers
{
    public class TimerMono : ATimerMono
    {
        private void Update()
        {
            if (!IsExecuting)
                return;

            timer.AddSeconds(Time.deltaTime);
        }
    }
}
