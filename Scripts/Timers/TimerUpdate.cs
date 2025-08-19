using UnityEngine;

#nullable enable
namespace UTIRLib.Timers
{
    public class TimerUpdate : TimerMono
    {
        private void Update()
        {
            if (!IsExecuting)
                return;

            timer.AddSeconds(Time.deltaTime);
        }

        public static ITimer Create()
        {
            return Create(UpdateType.Update);
        }
    }
}
