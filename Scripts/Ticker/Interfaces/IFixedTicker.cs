#nullable enable
using System;

namespace UTIRLib.AlternativeTicker
{
    public interface IFixedTicker : ITickerBase
    {
        IDisposable Register(IFixedTickable tickable);

        void Unregister(IFixedTickable tickable);
    }
}
