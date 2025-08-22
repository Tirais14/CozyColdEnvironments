#nullable enable
namespace UTIRLib.Tickables
{
    public abstract class TickerTimed<T> : Ticker<T>
        where T : ITickable
    {
        private readonly TickerTimeCounter timeCounter = new();
        private readonly LoopPredicate<TickerTimeCounter, float> tickPredicate = new()
        {
            Predicate = static (timer, deltaTime) => timer.IsTickAllowed(deltaTime)
        };

        protected void ProccessFrame(float deltaTime)
        {
            timeCounter.OnStartTick(deltaTime);

            while (tickPredicate.Invoke(timeCounter, deltaTime))
            {
                DoTickablesTicks(deltaTime);

                timeCounter.OnEndTick(deltaTime);
            }
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
    }
}
