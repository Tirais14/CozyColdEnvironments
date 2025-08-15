using System;
using System.Collections.Generic;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.AlternativeTicker
{
    public class LateTicker : MonoX, ILateTicker
    {
        private readonly List<ILateTickable> tickables = new();

        private void LateUpdate()
        {
            int count = tickables.Count;
            for (int i = 0; i < count; i++)
                tickables[i].LateTick();
        }

        private void OnDestroy() => ((IDisposable)this).Dispose();

        /// <exception cref="ArgumentNullException"></exception>
        public IDisposable Register(ILateTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Add(tickable);

            return Subscription.Create(this, tickable, (x, y) => x.Unregister(y));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Unregister(ILateTickable tickable)
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
