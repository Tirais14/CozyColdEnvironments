#nullable enable
using System;

namespace UTIRLib.Tickables
{
    public interface ILateTicker : ITickerBase
    {
        IDisposable Register(ILateTickable tickable);

        void Unregister(ILateTickable tickable);
    }
}
