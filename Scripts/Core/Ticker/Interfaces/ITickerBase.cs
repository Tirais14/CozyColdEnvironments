using System;

#nullable enable
namespace UTIRLib.AlternativeTicker
{
    public interface ITickerBase : IDisposable
    {
        void UnregisterAll();
    }
}
