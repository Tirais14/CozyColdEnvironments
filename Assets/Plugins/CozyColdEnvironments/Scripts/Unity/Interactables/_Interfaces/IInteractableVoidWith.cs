#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using UniRx;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableVoidWith : IInteractableWith
    {
        new void Interact(object tool);

        Maybe<object> IInteractableWith.Interact(object tool)
        {
            Interact(tool);

            return null!;
        }

        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            if (tool.IsNull())
                return ("tool required for interaction.", null);


            Interact(tool);
            return Either<string, object>.None;
        }
    }
    public interface IInteractableVoidWith<in T> 
        : IInteractableVoidWith,
        IObservable<Unit>
    {
        void Interact(T arg);

        void IInteractableVoidWith.Interact(object tool) => Interact(tool);

        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            if (tool.IsNull())
                return ($"tool (type: {typeof(T).GetFullName()}) required for interaction.", null);

            Interact(tool);
            return Either<string, object>.None;
        }
    }
}
