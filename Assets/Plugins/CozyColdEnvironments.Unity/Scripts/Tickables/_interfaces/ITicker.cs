using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Tickables
{
    public interface ITicker
    {
        TickableHandle Register(IFixedTickable fixedTickable);

        TickableHandle Register(ILateTickable lateTickable);

        TickableHandle Register(ITickable tickable);

        TickableHandle Register(object obj);

        bool Unregister(IFixedTickable? fixedTickable);

        bool Unregister(ILateTickable? lateTickable);

        bool Unregister(ITickable? tickable);

        bool Unregister(object? obj);

        bool Unregister(TickableHandle handle);
    }
}
