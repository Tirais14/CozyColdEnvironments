using System;
using System.Collections.Generic;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;
using UTIRLib.Patterns.Commands;
using UTIRLib.Reflection.Cached;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Tickables
{
    public abstract class Ticker<T> 
        :
        MonoX,
        ITicker<T>

        where T : ITickableBase
    {
        private int tickablesCount;

        protected readonly List<T> tickables = new();

        public float TickablesCount => tickablesCount;
        public bool IsEnabled => enabled;

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

        protected void DoTickablesTicks(float deltaTime)
        {
            for (int i = 0; i < tickablesCount; i++)
                DoTick(tickables[i]);
        }

        protected abstract void DoTick(T tickable);

        private void TryInjectTicker(T tickable)
        {
            FieldInfo? tickerField = TypeCache.GetField(tickable.GetType(),
                                                        GetType(),
                                                        BindingFlagsDefault.InstanceAll);

            if (tickerField is null)
                return;

            if (tickerField.IsInitOnly)
                throw new InvalidOperationException("Field with ticker cannot be readonly.");

            if (tickerField.GetValue(tickable).IsNotNull())
                throw new InvalidOperationException($"Field with type {nameof(ITicker).TrimFirst()} must be null(native or unity) before regsiterd.");

            tickerField.SetValue(tickable, this);
        }

        private void TryVoidInjectedTicker(T tickable)
        {
            FieldInfo? tickerField = TypeCache.GetField(tickable.GetType(),
                                                        GetType(),
                                                        BindingFlagsDefault.InstanceAll);

            if (tickerField is null)
                return;

            tickerField.SetValue(tickable, null);
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
