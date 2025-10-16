#nullable enable
namespace CCEnvs.Unity.Tickables
{
    public abstract class TickerTimed<T> : Ticker<T>
        where T : ITickableBase
    {
        private readonly TickerTimeCounter timeCounter = new();
        private readonly LoopFuse<TickerTimeCounter, float> tickPredicate = new()
        {
            Predicate = static (timer, deltaTime) => timer.IsTickAllowed(deltaTime)
        };

        private int framesProcessed;

        public override int FramesProcessed => framesProcessed;

        protected void ProccessFrames(float baseDeltaTime)
        {
            framesProcessed = 0;
            timeCounter.OnStartTick(baseDeltaTime);

            while (tickPredicate.Invoke(timeCounter, baseDeltaTime))
            {
                DoTickablesTicks();
                framesProcessed++;
                timeCounter.OnEndTick(baseDeltaTime);
            }

            DeltaTime = GetDeltaTime(baseDeltaTime);
        }

        public float TimeScale {
            get => timeCounter.TimeScale;
            set
            {
                if (value < 0)
                    value = 0;

                timeCounter.TimeScale = value;
            }
        }

        protected float GetDeltaTime(float baseDeltaTime)
        {
            if (framesProcessed <= 1)
                return baseDeltaTime;

            return baseDeltaTime * framesProcessed;
        }
    }
}
