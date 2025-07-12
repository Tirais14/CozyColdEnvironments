#nullable enable
using System;

namespace UTIRLib.AlternativeTicker
{
    public interface IFixedTicker : ITickerBase
    {
        void Register(IFixedTickable tickable);

        void Unregister(IFixedTickable tickable);
    }
}
