#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractable : IInteractableBase
    {
        object? Interact();

        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            return (null, Interact());
        }
    }
    public interface IInteractable<out T> 
        : IInteractable,
        IObservable<T>
    {
        new T Interact();

        object? IInteractable.Interact() => Interact();

        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            return (null, Interact());
        }
    }
}
