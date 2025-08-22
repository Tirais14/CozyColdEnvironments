#nullable enable
namespace UTIRLib.Tickables
{
    public sealed class TickerTimeCounter
    {
        private float accumulated;

        public float TimeScale { get; set; }

        public bool IsTickAllowed(float deltaTime)
        {
            return accumulated > deltaTime || accumulated.NearlyEquals(deltaTime);
        }

        public void OnStartTick(float deltaTime)
        {
            accumulated += deltaTime * TimeScale;
        }

        public void OnEndTick(float deltaTime)
        {
            accumulated -= deltaTime;
        }
    }
}
