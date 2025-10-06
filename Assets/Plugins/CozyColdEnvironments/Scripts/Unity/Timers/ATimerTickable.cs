using CCEnvs.Unity.Tickables;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S2933
namespace CCEnvs.Unity.Timers
{
    public abstract class ATimerTickable : TimerMono
    {
        private ITicker ticker = null!;

        protected ITicker Ticker => ticker;

        public static ITimer Create() => Create(UpdateType.Custom);

        public void DoTick() => Main();
    }
}
