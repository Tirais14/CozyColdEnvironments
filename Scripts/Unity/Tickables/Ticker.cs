using CCEnvs.Cacheables;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.Tickables
{
    public abstract class Ticker<T> 
        :
        MonoCC,
        ITicker<T>

        where T : ITickableBase
    {
        private int tickablesCount;

        protected readonly List<T> tickables = new();

        public float TickablesCount => tickablesCount;
        public float DeltaTime { get; protected set; }
        public bool IsEnabled => enabled;
        public virtual int FramesProcessed => 1;

        private void OnDestroy() => ((IDisposable)this).Dispose();

        public void Enable() => enabled = true;

        public void Disable() => enabled = false;

        /// <exception cref="ArgumentNullException"></exception>
        public IDisposable Register(T tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            TryInjectTicker(tickable);
            tickable.OnRegister();
            AddTickable(tickable);

            return Subscription.Create(this, tickable, (x, y) => x.Unregister(y));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public bool Unregister(T tickable)
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
            var loopPredicate = new LoopPredicate(() => tickablesCount > 0);
            while (loopPredicate)
                Unregister(tickables[^1]);
        }

        public bool IsRegistered(T tickable)
        {
            return tickables.Contains(tickable);
        }

        /// <summary>
        /// It's raw method. Not calculates <see cref="DeltaTime"/>
        /// </summary>
        protected void DoTickablesTicks()
        {
            T tickable;
            for (int i = 0; i < tickablesCount; i++)
            {
                tickable = tickables[i];
                if (tickable.IsTickableEnabled)
                    DoTick(tickable);
            }
        }

        protected abstract void DoTick(T tickable);

        private static void TryVoidInjectedTicker(T tickable)
        {
            Type tickableType = tickable.GetType();
            if (!TypeCache.Fields.TryGetValue(
                new FieldKey(tickableType, typeof(ITicker)),
                out FieldInfo? tickerField))
            {
                tickerField = tickableType.GetField(
                    typeof(ITicker),
                    BindingFlagsDefault.InstanceAll)
                    .TryCacheMember();
            }

            if (tickerField is null)
                return;

            tickerField.SetValue(tickable, null);
        }

        private void TryInjectTicker(T tickable)
        {
            Type tickableType = tickable.GetType();
            if (!TypeCache.Fields.TryGetValue(
                new FieldKey(tickableType, typeof(ITicker)),
                out FieldInfo? tickerField))
            {
                tickerField = tickableType.GetField(
                    typeof(ITicker),
                    BindingFlagsDefault.InstanceAll)
                    .TryCacheMember();
            }
            if (tickerField is null)
                return;

            if (tickerField.IsInitOnly)
                throw new InvalidOperationException("Field with ticker cannot be readonly.");

            if (tickerField.GetValue(tickable).IsNotNull())
                throw new InvalidOperationException($"Field with type {nameof(ITicker).TrimFirst()} must be null(native or unity) before regsiterd.");

            tickerField.SetValue(tickable, this);
        }

        private void AddTickable(T tickable)
        {
            tickables.Add(tickable);
            UpdateCount();
        }

        private void RemoveTickable(T tickable)
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
