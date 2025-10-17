#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractable : IPrioritized<int>, ILayerDependent
    {
        object? Interact();
    }
    public interface IInteractable<out T> 
        : IInteractable,
        IObservable<T>
    {
        new T Interact();

        object? IInteractable.Interact() => Interact();
    }
}
