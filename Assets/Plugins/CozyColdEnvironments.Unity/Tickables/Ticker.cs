using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.Tickables
{
    public abstract class Ticker<TTickable>
        :
        CCBehaviour,
        ITicker<TTickable>

        where TTickable : ITickableBase
    {
        protected readonly List<TTickable> tickables = new();

        private readonly ReactiveProperty<bool> isEnabled = new();

        private int tickablesCount;

        public float TickablesCount => tickablesCount;
        public float DeltaTime { get; protected set; }
        public bool IsEnabled => isEnabled.Value;
        public virtual int FramesProcessed => 1;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ((IDisposable)this).Dispose();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isEnabled.Value = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            isEnabled.Value = false;
        }

        public void Enable()
        {
            if (isEnabled.Value)
                return;

            enabled = true;
        }

        public void Disable()
        {
            if (!isEnabled.Value)
                return;

            enabled = false;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public IDisposable Register(TTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            TryInjectTicker(tickable);
            tickable.OnRegister();
            AddTickable(tickable);

            return Subscription.Create(this, tickable, (x, y) => x.Unregister(y));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public bool Unregister(TTickable tickable)
        {
            if (tickable.IsNull())
                return false;

            tickable.OnUnregister();
            TryVoidInjectedTicker(tickable);

            if (tickable is IDisposable disposable)
                disposable.Dispose();

            RemoveTickable(tickable);
            return true;
        }

        public void UnregisterAll()
        {
            var loopFuse = LoopFuse.Create();

            while (loopFuse.MoveNext() && tickablesCount > 0)
                Unregister(tickables[^1]);
        }

        public bool IsRegistered(TTickable tickable)
        {
            return tickables.Contains(tickable);
        }

        public Observable<bool> ObserveEnabled()
        {
            return isEnabled.Where(static x => x);
        }

        public Observable<bool> ObserveDisabled()
        {
            return isEnabled.Where(static x => !x);
        }

        /// <summary>
        /// It's raw method. Not calculates <see cref="DeltaTime"/>
        /// </summary>
        protected void DoTickablesTicks()
        {
            TTickable tickable;
            for (int i = 0; i < tickablesCount; i++)
            {
                tickable = tickables[i];
                if (tickable.IsTickableEnabled)
                    DoTick(tickable);
            }
        }

        protected abstract void DoTick(TTickable tickable);

        private static void TryVoidInjectedTicker(TTickable tickable)
        {
            tickable.Reflect()
                    .IncludeNonPublic()
                    .IncludeBaseTypes()
                    .WithTypeFilter<ITicker>()
                    .Cache()
                    .Field()
                    .Lax()
                    .IfSome(prop => prop.SetValue(tickable, null));
        }

        private void TryInjectTicker(TTickable tickable)
        {
            tickable.Reflect()
                    .IncludeNonPublic()
                    .IncludeBaseTypes()
                    .WithTypeFilter<ITicker>()
                    .Cache()
                    .Field()
                    .Lax()
                    .IfSome(prop => prop.SetValue(tickable, this));
        }

        private void AddTickable(TTickable tickable)
        {
            tickables.Add(tickable);
            UpdateCount();
        }

        private void RemoveTickable(TTickable tickable)
        {
            tickables.Remove(tickable);
            UpdateCount();
        }

        private void UpdateCount()
        {
            tickablesCount = tickables.Count;
        }

        void IDisposable.Dispose()
        {
            UnregisterAll();
            GC.SuppressFinalize(this);
        }
    }
}
