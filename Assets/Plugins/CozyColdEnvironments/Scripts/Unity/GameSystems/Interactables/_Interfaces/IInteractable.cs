#nullable enable
using System;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractable : IInteractableBase
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
