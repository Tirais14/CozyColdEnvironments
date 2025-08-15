#nullable enable
using System;
using System.Collections.Generic;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;

#pragma warning disable S3881
namespace UTIRLib.AlternativeTicker
{
    public class FixedTicker : MonoX, IFixedTicker
    {
        private readonly List<IFixedTickable> tickables = new();

        private void FixedUpdate()
        {
            int count = tickables.Count;
            for (int i = 0; i < count; i++)
                tickables[i].FixedTick();
        }

        private void OnDestroy() => ((IDisposable)this).Dispose();

        /// <exception cref="ArgumentNullException"></exception>
        public IDisposable Register(IFixedTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable)); 

            tickables.Add(tickable);

            return Subscription.Create(this, tickable, (x, y) => x.Unregister(y));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Unregister(IFixedTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Remove(tickable); 
        }

        public void UnregisterAll() => tickables.Clear();

        void IDisposable.Dispose()
        {
            UnregisterAll();
            GC.SuppressFinalize(this);
        }
    }
}
