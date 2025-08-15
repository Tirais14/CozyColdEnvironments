using System;

#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickerBase : IDisposable
    {
        void UnregisterAll();
    }
}
