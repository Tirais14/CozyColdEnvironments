using System;
using System.Collections.Generic;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public class Ticker : MonoX, ITicker
    {
        private readonly List<ITickable> tickables = new();

        /// <exception cref="ArgumentNullException"></exception>
        public void Register(ITickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Add(tickable);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Unregister(ITickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Remove(tickable);
        }

        public void UnregisterAll() => tickables.Clear();

        public void Dispose() => UnregisterAll();

        private void Update()
        {
            int count = tickables.Count;
            for (int i = 0; i < count; i++)
                tickables[i].Tick();
        }
    }
}
