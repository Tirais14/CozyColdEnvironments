#nullable enable
using System;

namespace UTIRLib.AlternativeTicker
{
    public interface ITicker : ITickerBase
    {
        void Register(ITickable tickable);

        void Unregister(ITickable tickable);
    }
}
