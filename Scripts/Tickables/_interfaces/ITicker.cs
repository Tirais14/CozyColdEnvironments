using System;
using CozyColdEnvironments.Reflection;
using CozyColdEnvironments.Unity.TypeMatching;

#nullable enable
namespace CozyColdEnvironments.Tickables
{
    public interface ITicker : ISwitchable, IDisposable
    {
        float DeltaTime { get; }
        int FramesProcessed { get; }

        bool IsRegistered(ITickableBase tickable);

        IDisposable Register(ITickableBase tickable);

        bool Unregister(ITickableBase tickable);

        void UnregisterAll();
    }
    public interface ITicker<T> : ITicker
        where T : ITickableBase
    {
        bool IsRegistered(T tickable);

        IDisposable Register(T tickable);

        bool Unregister(T tickable);

        bool ITicker.IsRegistered(ITickableBase tickable)
        {
            if (tickable.IsNot<T>())
                return false;

            return IsRegistered(tickable);
        }

        IDisposable ITicker.Register(ITickableBase tickable)
        {
            if (tickable.IsNot<T>(out var typed))
                throw new ArgumentException($"Expected tickable type = {typeof(T).GetName()}.");

            return Register(typed);
        }

        bool ITicker.Unregister(ITickableBase? tickable)
        {
            if (tickable.IsNot<T>(out var typed))
                return false;

            return Unregister(typed);
        }
    }
}
