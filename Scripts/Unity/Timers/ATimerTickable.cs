using CCEnvs.Unity.Tickables;
using System;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S2933
namespace CCEnvs.Unity.Timers
{
    [Obsolete("In developing")]
    public abstract class ATimerTickable : TimerMono
    {
        private ITicker ticker = null!;

        protected ITicker Ticker => ticker;
        protected override float DeltaTime => ticker.DeltaTime;

        public static ITimer Create(ITicker ticker) => Create(UpdateType.Custom);

        public void DoTick() => Main();
    }
}
