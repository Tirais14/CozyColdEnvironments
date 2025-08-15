#nullable enable
using System;

namespace UTIRLib.AlternativeTicker
{
    public interface ILateTicker : ITickerBase
    {
        IDisposable Register(ILateTickable tickable);

        void Unregister(ILateTickable tickable);
    }
}
