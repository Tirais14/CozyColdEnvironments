#nullable enable
using UTIRLib.Diagnostics;
using System;
using System.Collections.Generic;

namespace UTIRLib.AlternativeTicker
{
    public class FixedTicker : MonoX, IFixedTicker
    {
        private readonly List<IFixedTickable> tickables = new();

        /// <exception cref="ArgumentNullException"></exception>
        public void Register(IFixedTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable)); 

            tickables.Add(tickable);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Unregister(IFixedTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Remove(tickable); 
        }

        public void UnregisterAll() => throw new System.NotImplementedException();

        public void Dispose() => throw new System.NotImplementedException();

        private void FixedUpdate()
        {
            int count = tickables.Count;
            for (int i = 0; i < count; i++)
                tickables[i].FixedTick();
        }
    }
}
