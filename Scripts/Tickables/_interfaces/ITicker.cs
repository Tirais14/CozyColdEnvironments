using System;
using UTIRLib.Reflection;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITicker : ISwitchable, IDisposable
    {
        IDisposable Register(ITickableBase tickable);

        bool Unregister(ITickableBase tickable);

        void UnregisterAll();
    }
    public interface ITicker<T> : ITicker
        where T : ITickableBase
    {
        IDisposable Register(T tickable);

        bool Unregister(T tickable);

        IDisposable ITicker.Register(ITickableBase tickable)
        {
            if (tickable.IsNot<T>(out var typed))
                throw new ArgumentException($"Expected tickable type = {typeof(T).GetName()}.");

            return Register(typed);
        }

        bool ITicker.Unregister(ITickableBase? tickable)
        {
            if (tickable.IsNot<T>(out var typed))
                return;

            return Unregister(typed);
        }
    }
}
