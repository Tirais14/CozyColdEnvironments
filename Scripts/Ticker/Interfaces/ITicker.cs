#nullable enable
using System;

namespace UTIRLib.AlternativeTicker
{
    public interface ITicker : ITickerBase
    {
        IDisposable Register(ITickable tickable);

        void Unregister(ITickable tickable);
    }
}
