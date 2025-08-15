#nullable enable
using System;

namespace UTIRLib.Tickables
{
    public interface ITicker : ITickerBase
    {
        IDisposable Register(ITickable tickable);

        void Unregister(ITickable tickable);
    }
}
