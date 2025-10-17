using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IInteractableWith : IPrioritized<int>, ILayerDependent
    {
        object? Interact(object arg);
    }
    public interface IInteractableWith<in TIn, out TOut>
        : IInteractableWith,
        IObservable<TOut>
    {
        TOut Interact(TIn arg);

        object? IInteractableWith.Interact(object arg) => Interact(arg);
    }
}
