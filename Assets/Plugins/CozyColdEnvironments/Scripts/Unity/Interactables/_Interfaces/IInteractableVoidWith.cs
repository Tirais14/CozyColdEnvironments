#nullable enable
using System;
using UniRx;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableVoidWith : IInteractableWith
    {
        new void Interact(object arg);

        object? IInteractableWith.Interact(object arg)
        {
            Interact(arg);

            return null;
        }
    }
    public interface IInteractableVoidWith<in T> 
        : IInteractableVoidWith,
        IObservable<Unit>
    {
        void Interact(T arg);

        void IInteractableVoidWith.Interact(object arg) => Interact(arg);
    }
}
