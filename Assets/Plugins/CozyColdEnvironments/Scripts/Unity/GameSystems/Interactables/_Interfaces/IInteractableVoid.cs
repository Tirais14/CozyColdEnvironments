#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractableVoid
        : IInteractable,
        IObservable<Unit>
    {
        new void Interact();

        object? IInteractable.Interact() => null;
    }
}
