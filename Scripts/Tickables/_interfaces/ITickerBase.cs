using System;
using UTIRLib.Patterns.Commands;

#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickerBase : IDisposable
    {
        void UnregisterAll();

        void SendCommand(ICommandX command);
    }
    public interface ITickerBase<T> : ITickerBase
        where T : ITickableBase
    {
        IDisposable Register(T tickable);

        void Unregister(T tickable);
    }
}
