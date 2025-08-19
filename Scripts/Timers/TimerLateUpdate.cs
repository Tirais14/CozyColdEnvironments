using UnityEngine;

#nullable enable
namespace UTIRLib.Timers
{
    public class TimerLateUpdate : TimerMono
    {
        private void LateUpdate()
        {
            if (!IsExecuting)
                return;

            timer.AddSeconds(Time.deltaTime);
        }

        public static ITimer Create()
        {
            return Create(UpdateType.LateUpdate);
        }
    }
}
