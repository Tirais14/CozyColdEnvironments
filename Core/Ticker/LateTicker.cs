using System;
using System.Collections.Generic;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public class LateTicker : MonoX, ILateTicker
    {
        private readonly List<ILateTickable> tickables = new();

        /// <exception cref="ArgumentNullException"></exception>
        public void Register(ILateTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Add(tickable);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Unregister(ILateTickable tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickables.Remove(tickable);
        }

        public void UnregisterAll() => tickables.Clear();

        public void Dispose() => UnregisterAll();

        private void LateUpdate()
        {
            int count = tickables.Count;
            for (int i = 0; i < count; i++)
                tickables[i].LateTick();
        }
    }
}
