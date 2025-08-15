#nullable enable
using System;

namespace UTIRLib.Tickables
{
    public interface IFixedTicker : ITickerBase
    {
        IDisposable Register(IFixedTickable tickable);

        void Unregister(IFixedTickable tickable);
    }
}
